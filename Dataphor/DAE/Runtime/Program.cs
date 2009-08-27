﻿/*
	Alphora Dataphor
	© Copyright 2000-2009 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Alphora.Dataphor.DAE.Runtime
{
	using Alphora.Dataphor.DAE.Language;
	using Alphora.Dataphor.DAE.Language.D4;
	using Alphora.Dataphor.DAE.Compiling;
	using Alphora.Dataphor.DAE.Debug;
	using Alphora.Dataphor.DAE.Device.Catalog;
	using Alphora.Dataphor.DAE.Server;
	using Alphora.Dataphor.DAE.Streams;
	using Alphora.Dataphor.DAE.Runtime.Data;
	using Alphora.Dataphor.DAE.Runtime.Instructions;

	/// <summary>
	/// Represents the run-time aspects of a compiled D4 program.
	/// </summary>
	public class Program
	{
		public Program(ServerProcess AProcess) : this(AProcess, Guid.NewGuid()) { }
		public Program(ServerProcess AProcess, Guid AID)
		{
			FServerProcess = AProcess;
			FID = AID;
			FStack = new Stack(FServerProcess.MaxStackDepth, FServerProcess.MaxCallDepth);
		}
		
		private ServerProcess FServerProcess;
		public ServerProcess ServerProcess { get { return FServerProcess; } }
		
		private bool FIsCached;
		public bool IsCached
		{
			get { return FIsCached; }
			set { FIsCached = value; }
		}
		
		public void BindToProcess(ServerProcess AProcess, Plan APlan)
		{
			FServerProcess = AProcess;
			FCode.BindToProcess(APlan);
			
			// Reset execution time
			FStatistics.ExecuteTime = TimeSpan.Zero;
			FStatistics.DeviceExecuteTime = TimeSpan.Zero;
		}
		
		private Guid FID;
		public Guid ID { get { return FID; } }
		
		public int DefaultMaxStackDepth
		{
			get { return FServerProcess.ServerSession.SessionInfo.DefaultMaxStackDepth; }
			set { FServerProcess.ServerSession.SessionInfo.DefaultMaxStackDepth = value; }
		}
		
		public int DefaultMaxCallDepth
		{
			get { return FServerProcess.ServerSession.SessionInfo.DefaultMaxCallDepth; }
			set { FServerProcess.ServerSession.SessionInfo.DefaultMaxCallDepth = value; }
		}
		
		private Stack FStack;
		public Stack Stack { get { return FStack; } }
		
		public Stack SwitchContext(Stack AContext)
		{
			Stack LContext = FStack;
			FStack = AContext;
			return LContext;
		}

		private ProgramStatistics FStatistics = new ProgramStatistics();
		public ProgramStatistics Statistics { get { return FStatistics; } }

		// Code
		protected PlanNode FCode;
		public PlanNode Code
		{
			get { return FCode; }
			set { FCode = value; }
		}
		
		// DataType
		protected Schema.IDataType FDataType;
		public Schema.IDataType DataType 
		{ 
			get { return FDataType; }
			set { FDataType = value; } 
		}
		
		// ProcessLocals - New local variables declared by allocation statements in the program
		private DataParams FProcessLocals = new DataParams();
		public DataParams ProcessLocals { get { return FProcessLocals; } }
		
		// Used to track the set of process local variables pushed when the program was started.
		private DataParams FLocalParams;
		
		// Source
		protected string FSource;
		/// <summary>
		/// Contains the source text for the plan. Only present if no debug locator is provided.
		/// </summary>
		public string Source { get { return FSource; } }
		
		// Locator
		protected DebugLocator FLocator;
		/// <summary>
		/// Provides a reference for identifying the source text for the plan. May be null for dynamic or ad-hoc execution.
		/// </summary>
		public DebugLocator Locator { get { return FLocator; } }
		
		/// <summary>
		/// Sets the source context for the plan.
		/// </summary>
		public void SetSourceContext(SourceContext ASourceContext)
		{
			// Clear existing context
			FSource = null;
			FLocator = null;
			
			if (ASourceContext.Locator != null)
				FLocator = ASourceContext.Locator;
			else
			{
				FLocator = new DebugLocator(DebugLocator.PlanLocator(this.ID), 1, 1);
				FSource = ASourceContext.Script;
			}
		}
		
		// Devices
		public Schema.DeviceSession DeviceConnect(Schema.Device ADevice)
		{
			return FServerProcess.DeviceConnect(ADevice);
		}
		
		public object DeviceExecute(Schema.Device ADevice, PlanNode APlanNode)
		{	
			if (FServerProcess.IsReconciliationEnabled() || (APlanNode.DataType != null))
			{
				long LStartTicks = TimingUtility.CurrentTicks;
				try
				{
					return DeviceConnect(ADevice).Execute(this, Plan.GetDevicePlan(APlanNode));
				}
				finally
				{
					FStatistics.DeviceExecuteTime += TimingUtility.TimeSpanFromTicks(LStartTicks);
				}
			}

			return null;
		}
		
		// Remote Sessions
		public RemoteSession RemoteConnect(Schema.ServerLink ALink)
		{
			return FServerProcess.RemoteConnect(ALink);
		}
		
		// Plan
		private Plan FPlan;
		public Plan Plan 
		{ 
			get 
			{ 
				if (FPlan == null)
					FPlan = new Plan(FServerProcess);
				return FPlan;
			}
		}
		
		public Schema.LoadedLibrary CurrentLibrary { get { return Plan.CurrentLibrary; } }
		
		public Schema.User User { get { return Plan.User; } }
		
		// Catalog
		public Schema.Catalog Catalog { get { return FServerProcess.Catalog; } }
		
		public CatalogDeviceSession CatalogDeviceSession { get { return FServerProcess.CatalogDeviceSession; } }
		
		public Schema.DataTypes DataTypes { get { return FServerProcess.DataTypes; } }
		
		public Schema.Device TempDevice { get { return FServerProcess.ServerSession.Server.TempDevice; } }
		
		// Values
		public IValueManager ValueManager { get { return FServerProcess.ValueManager; } }
		
		// Streams
		public IStreamManager StreamManager { get { return FServerProcess.StreamManager; } }
		
		// Cursors
		public CursorManager CursorManager { get { return FServerProcess.ServerSession.CursorManager; } }

		// Execution
		public void Start(DataParams AParams)
		{
			FStack.PushWindow(0, null, Locator);
			FServerProcess.PushExecutingProgram(this);
			try
			{
				FLocalParams = new DataParams();
				DataParams LParams = new DataParams();
				foreach (DataParam LParam in FServerProcess.ProcessLocals)
					if (!ProcessLocals.Contains(LParam.Name))
					{
						FLocalParams.Add(LParam);
						LParams.Add(LParam);
					}
				
				if (AParams != null)
					foreach (DataParam LParam in AParams)
						LParams.Add(LParam);
						
				foreach (DataParam LParam in LParams)
					FStack.Push(LParam.Modifier == Modifier.In ? DataValue.CopyValue(ValueManager, LParam.Value) : LParam.Value);
			}
			catch
			{
				FServerProcess.PopExecutingProgram(this);
				FStack.PopWindow();
				throw;
			}
		}
		
		public void Stop(DataParams AParams)
		{
			try
			{
				DataParams LParams = new DataParams();
				foreach (DataParam LParam in FLocalParams)
					LParams.Add(LParam);
					
				if (AParams != null)
					foreach (DataParam LParam in AParams)
						LParams.Add(LParam);
						
				for (int LIndex = ProcessLocals.Count - 1; LIndex >= 0; LIndex--)
				{
					ProcessLocals[LIndex].Value = FStack.Pop();
					FServerProcess.AddProcessLocal(ProcessLocals[LIndex]);
				}
						
				for (int LIndex = LParams.Count - 1; LIndex >= 0; LIndex--)
				{
					object LValue = FStack.Pop();
					if (LParams[LIndex].Modifier != Modifier.In)
						LParams[LIndex].Value = LValue;
				}
			}
			finally
			{
				FServerProcess.PopExecutingProgram(this);
			}
		}
		
		public object Execute(DataParams AParams)
		{	
			object LResult;
			#if TRACEEVENTS
			RaiseTraceEvent(TraceCodes.BeginExecute, "Begin Execute");
			#endif
			Start(AParams);
			try
			{
				long LStartTicks = TimingUtility.CurrentTicks;
				LResult = FCode.Execute(this);
				FStatistics.ExecuteTime = TimingUtility.TimeSpanFromTicks(LStartTicks);
			}
			finally
			{
				Stop(AParams);
			}
			#if TRACEEVENTS
			RaiseTraceEvent(TraceCodes.EndExecute, "End Execute");
			#endif
			return LResult;
		}
		
		private PlanNode FCurrentNode;
		
		public void Yield(PlanNode APlanNode)
		{
			if (FServerProcess.IsAborted)
				throw new ServerException(ServerException.Codes.ProcessAborted);

			if (FServerProcess.Debugger != null)
			{
				FCurrentNode = APlanNode;
				FServerProcess.Debugger.Yield(FServerProcess, APlanNode, null);
			}
		}
		
		public void Yield(PlanNode APlanNode, Exception AException)
		{
			if (FServerProcess.IsAborted)
				throw new ServerException(ServerException.Codes.ProcessAborted);

			if (FServerProcess.Debugger != null)
			{
				FCurrentNode = APlanNode;
				FServerProcess.Debugger.Yield(FServerProcess, APlanNode, AException);
			}
		}
		
		public void CheckAborted()
		{
			if (FServerProcess.IsAborted)
				throw new ServerException(ServerException.Codes.ProcessAborted);
		}
		
		public DebugLocator GetCurrentLocation()
		{
			try
			{
				// Current location is the line/linepos of the current node, with the locator as the current locator on the call stack.
				return new DebugLocator(((RuntimeStackWindow)FStack.CurrentStackWindow).Locator, FCurrentNode == null ? -1 : FCurrentNode.Line, FCurrentNode == null ? -1 : FCurrentNode.LinePos);
			}
			catch (Exception E)
			{
				throw new ServerException(ServerException.Codes.CouldNotDetermineProgramLocation, E, FID);
			}
		}
		
		public DebugLocator SafeGetCurrentLocation()
		{
			try
			{
				return GetCurrentLocation();
			}
			catch (Exception E)
			{
				return new DebugLocator(E.Message, -1, -1);
			}
		}
		
		// Run-time Compilation
		public void EnsureKey(Schema.TableVar ATableVar)
		{
			Compiler.EnsureKey(Plan, ATableVar);
		}
		
		public Schema.Key FindKey(Schema.TableVar ATableVar, KeyDefinitionBase AKeyDefinition)
		{
			return Compiler.FindKey(Plan, ATableVar, AKeyDefinition);
		}
		
		public Schema.Key FindClusteringKey(Schema.TableVar ATableVar)
		{
			return Compiler.FindClusteringKey(Plan, ATableVar);
		}
		
		public Schema.Order OrderFromKey(Schema.Key AKey)
		{
			return Compiler.OrderFromKey(Plan, AKey);
		}
		
		public Schema.Order FindClusteringOrder(Schema.TableVar ATableVar)
		{
			return Compiler.FindClusteringOrder(Plan, ATableVar);
		}
		
		public Schema.Object ResolveCatalogObjectSpecifier(string ASpecifier)
		{
			return Compiler.ResolveCatalogObjectSpecifier(Plan, ASpecifier);
		}
		
		public Schema.Object ResolveCatalogObjectSpecifier(string ASpecifier, bool AMustResolve)
		{
			return Compiler.ResolveCatalogObjectSpecifier(Plan, ASpecifier, AMustResolve);
		}
		
		public Schema.Object ResolveCatalogIdentifier(string AIdentifier)
		{
			return Compiler.ResolveCatalogIdentifier(Plan, AIdentifier);
		}

		public Schema.Object ResolveCatalogIdentifier(string AIdentifier, bool AMustResolve)
		{
			return Compiler.ResolveCatalogIdentifier(Plan, AIdentifier, AMustResolve);
		}
	}
	
	public class Programs : List<Program> { }
}