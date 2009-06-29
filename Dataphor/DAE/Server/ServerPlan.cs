﻿/*
	Alphora Dataphor
	© Copyright 2000-2009 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
*/

using System;
using System.Text;
using System.Collections.Generic;

using Alphora.Dataphor.DAE.Language;
using Alphora.Dataphor.DAE.Language.D4;
using Alphora.Dataphor.DAE.Runtime;
using Alphora.Dataphor.DAE.Runtime.Data;
using Alphora.Dataphor.DAE.Runtime.Instructions;

namespace Alphora.Dataphor.DAE.Server
{
	public abstract class ServerPlanBase : ServerChildObject, IServerPlanBase
	{        
		protected internal ServerPlanBase(ServerProcess AProcess) : base()
		{
			FProcess = AProcess;
			FPlan = new Plan(AProcess);

			#if !DISABLE_PERFORMANCE_COUNTERS
			if (FProcess.ServerSession.Server.FPlanCounter != null)
				FProcess.ServerSession.Server.FPlanCounter.Increment();
			#endif
		}
		
		private bool FDisposed;
		
		protected override void Dispose(bool ADisposing)
		{
			try
			{
				try
				{
					if (FActiveCursor != null)
						FActiveCursor.Dispose();
				}
				finally
				{
					if (FPlan != null)
					{
						FPlan.Dispose();
						FPlan = null;
					}
					
					if (!FDisposed)
					{
						#if !DISABLE_PERFORMANCE_COUNTERS
						if (FProcess.ServerSession.Server.FPlanCounter != null)
							FProcess.ServerSession.Server.FPlanCounter.Decrement();
						#endif
						FDisposed = true;
					}
				}
			}
			finally
			{
				FCode = null;
				FDataType = null;
				FProcess = null;
	            
				base.Dispose(ADisposing);
			}
		}

		// ID		
		private Guid FID = Guid.NewGuid();
		public Guid ID { get { return FID; } }
		
		// CachedPlanHeader
		private CachedPlanHeader FHeader;
		public CachedPlanHeader Header
		{ 
			get { return FHeader; } 
			set { FHeader = value; }
		}
		
		// PlanCacheTimeStamp
		public long PlanCacheTimeStamp; // Server.PlanCacheTimeStamp when the plan was compiled

		// Plan		
		private Plan FPlan;
		public Plan Plan { get { return FPlan; } }
		
		// Statistics
		public PlanStatistics Statistics { get { return FPlan.Statistics; } }
		
		// Code
		protected PlanNode FCode;
		public PlanNode Code
		{
			get { return FCode; }
			set { FCode = value; }
		}
		
		// Process        
		protected ServerProcess FProcess;
		public ServerProcess ServerProcess { get { return FProcess; } }
		
		public void BindToProcess(ServerProcess AProcess)
		{
			FProcess = AProcess;
			FPlan.BindToProcess(AProcess);
			FCode.BindToProcess(AProcess.Plan);
		}
		
		// ActiveCursor
		protected ServerCursorBase FActiveCursor;
		public ServerCursorBase ActiveCursor { get { return FActiveCursor; } }
		
		public void SetActiveCursor(ServerCursorBase AActiveCursor)
		{
			if (FActiveCursor != null)
				throw new ServerException(ServerException.Codes.PlanCursorActive);
			FActiveCursor = AActiveCursor;
			FProcess.SetActiveCursor(FActiveCursor);
		}
		
		public void ClearActiveCursor()
		{
			FActiveCursor = null;
			FProcess.ClearActiveCursor();
		}
		
		protected Schema.IDataType FDataType;
		public Schema.IDataType DataType 
		{ 
			get 
			{ 
				CheckCompiled();
				return FDataType; 
			} 
			set { FDataType = value; } 
		}
		
		public virtual Statement EmitStatement()
		{
			CheckCompiled();
			return FCode.EmitStatement(EmitMode.ForCopy);
		}

		public void WritePlan(System.Xml.XmlWriter AWriter)
		{
			CheckCompiled();
			FCode.WritePlan(AWriter);
		}
		
		protected Exception WrapException(Exception AException)
		{
			return FProcess.ServerSession.WrapException(AException);
		}
		
		public void CheckCompiled()
		{
			FPlan.CheckCompiled();
		}
	}
	
	// ServerPlans    
	public class ServerPlans : ServerChildObjects
	{		
		protected override void Validate(ServerChildObject AObject)
		{
			if (!(AObject is ServerPlanBase))
				throw new ServerException(ServerException.Codes.ServerPlanContainer);
		}
		
		public new ServerPlanBase this[int AIndex]
		{
			get { return (ServerPlanBase)base[AIndex]; }
			set { base[AIndex] = value; }
		}
	}

	// ServerStatementPlan	
	public class ServerStatementPlan : ServerPlan, IServerStatementPlan
	{
		public ServerStatementPlan(ServerProcess AProcess) : base(AProcess) {}
		
		public void Execute(DataParams AParams)
		{
			Exception LException = null;
			int LNestingLevel = FProcess.BeginTransactionalCall();
			try
			{
				CheckCompiled();
				FProcess.Execute(this, FCode, AParams);
			}
			catch (Exception E)
			{
				LException = E;
				throw WrapException(E);
			}
			finally
			{
				FProcess.EndTransactionalCall(LNestingLevel, LException);
			}
		}
	}
	
	// ServerExpressionPlan
	public class ServerExpressionPlan : ServerPlan, IServerExpressionPlan
	{
		protected internal ServerExpressionPlan(ServerProcess AProcess) : base(AProcess) {}
		
		public DataValue Evaluate(DataParams AParams)
		{
			Exception LException = null;
			int LNestingLevel = FProcess.BeginTransactionalCall();
			try
			{
				CheckCompiled();
				return ServerProcess.Execute(this, Code, AParams).Value;
			}
			catch (Exception E)
			{
				LException = E;
				throw WrapException(LException);
			}
			finally
			{
				FProcess.EndTransactionalCall(LNestingLevel, LException);
			}
		}

		public IServerCursor Open(DataParams AParams)
		{
			IServerCursor LServerCursor;
			//ServerProcess.RaiseTraceEvent(TraceCodes.BeginOpenCursor, "Begin Open Cursor");
			Exception LException = null;
			int LNestingLevel = FProcess.BeginTransactionalCall();
			try
			{
				CheckCompiled();

				#if TIMING
				DateTime LStartTime = DateTime.Now;
				System.Diagnostics.Debug.WriteLine(String.Format("{0} -- ServerExpressionPlan.Open", DateTime.Now.ToString("hh:mm:ss.ffff")));
				#endif
				ServerCursor LCursor = new ServerCursor(this, AParams);
				try
				{
					LCursor.Open();
					#if TIMING
					System.Diagnostics.Debug.WriteLine(String.Format("{0} -- ServerExpressionPlan.Open -- Open Time: {1}", DateTime.Now.ToString("hh:mm:ss.ffff"), (DateTime.Now - LStartTime).ToString()));
					#endif
					LServerCursor = (IServerCursor)LCursor;
				}
				catch
				{
					Close((IServerCursor)LCursor);
					throw;
				}
			}
			catch (Exception E)
			{
				if (Header != null)
					Header.IsInvalidPlan = true;

				LException = E;
				throw WrapException(E);
			}
			finally
			{
				FProcess.EndTransactionalCall(LNestingLevel, LException);
			}
			//ServerProcess.RaiseTraceEvent(TraceCodes.EndOpenCursor, "End Open Cursor");
			return LServerCursor;
		}
		
		public void Close(IServerCursor ACursor)
		{
			Exception LException = null;
			int LNestingLevel = FProcess.BeginTransactionalCall();
			try
			{
				((ServerCursor)ACursor).Dispose();
			}
			catch (Exception E)
			{
				LException = E;
				throw WrapException(E);
			}
			finally
			{
				FProcess.EndTransactionalCall(LNestingLevel, LException);
			}
		}
		
		private Schema.TableVar FTableVar;
		Schema.TableVar IServerExpressionPlan.TableVar 
		{ 
			get 
			{ 
				CheckCompiled();
				if (FTableVar == null)
					FTableVar = (Schema.TableVar)Plan.PlanCatalog[Schema.Object.NameFromGuid(ID)];
				return FTableVar; 
			} 
		}
		
		Schema.Catalog IServerExpressionPlan.Catalog { get { return Plan.PlanCatalog; } }
		
		public Row RequestRow()
		{
			CheckCompiled();
			return new Row(FProcess, ((Schema.TableType)DataType).RowType);
		}
		
		public void ReleaseRow(Row ARow)
		{
			CheckCompiled();
			ARow.Dispose();
		}
		
		public override Statement EmitStatement()
		{
			CheckCompiled();
			return FCode.EmitStatement(EmitMode.ForRemote);
		}

		// SourceNode
		private TableNode SourceNode { get { return (TableNode)Code.Nodes[0]; } }
        
		// Isolation
		public CursorIsolation Isolation 
		{
			get 
			{ 
				CheckCompiled();
				return SourceNode.CursorIsolation; 
			}
		}
		
		// CursorType
		public CursorType CursorType 
		{ 
			get 
			{ 
				CheckCompiled();
				return SourceNode.CursorType; 
			} 
		}

		// Capabilities		
		public CursorCapability Capabilities 
		{ 
			get 
			{ 
				CheckCompiled();
				return SourceNode.CursorCapabilities; 
			} 
		}

		public bool Supports(CursorCapability ACapability)
		{
			return (Capabilities & ACapability) != 0;
		}
		
		// Order
		public Schema.Order Order
		{
			get
			{
				CheckCompiled();
				return SourceNode.Order;
			}
		}
	}
}