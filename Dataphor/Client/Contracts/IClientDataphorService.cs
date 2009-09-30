﻿/*
	Alphora Dataphor
	© Copyright 2000-2008 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
*/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;

namespace Alphora.Dataphor.DAE.Contracts
{
	using Alphora.Dataphor.DAE.Runtime;
	using Alphora.Dataphor.DAE.Server;
	using Alphora.Dataphor.DAE.Streams;
	using Alphora.Dataphor.DAE.Debug;

	[ServiceContract(Name = "IDataphorService", Namespace = "http://dataphor.org/dataphor/3.0/")]
	public interface IClientDataphorService
	{
		// Server
		#region Server
		
		/// <summary>
		/// Returns the name of the server.
		/// </summary>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetServerName(AsyncCallback ACallback, object AState);
		string EndGetServerName(IAsyncResult AResult);
		
		/// <summary>
		/// Returns the current cache timestamp of the catalog.
		/// </summary>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetCacheTimeStamp(AsyncCallback ACallback, object AState);
		long EndGetCacheTimeStamp(IAsyncResult AResult);
		
		/// <summary>
		/// Returns the current derivation timestamp of the catalog.
		/// </summary>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetDerivationTimeStamp(AsyncCallback ACallback, object AState);
		long EndGetDerivationTimeStamp(IAsyncResult AResult);
		
		#endregion
		
		// Session
		#region Session

		/// <summary>
		/// Initiates a connection to the Dataphor server.
		/// </summary>
		/// <param name="ASessionInfo">The session information used to authenticate and describe the session.</param>
		/// <returns>A session descriptor that describes the new session.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginConnect(SessionInfo ASessionInfo, AsyncCallback ACallback, object AState);
		SessionDescriptor EndConnect(IAsyncResult AResult);

		/// <summary>
		/// Disconnects an active Dataphor session.
		/// </summary>
		/// <param name="ASessionHandle">The handle to the session to be disconnected.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginDisconnect(int ASessionHandle, AsyncCallback ACallback, object AState);
		void EndDisconnect(IAsyncResult AResult);
		
		#endregion
		
		// Process
		#region Process
		
		/// <summary>
		/// Starts a server process.
		/// </summary>
		/// <param name="ASessionHandle">The handle of the session that will be used to start the process.</param>
		/// <param name="AProcessInfo">The process information used to describe the new process.</param>
		/// <returns>A process descriptor that describes the new process.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginStartProcess(int ASessionHandle, ProcessInfo AProcessInfo, AsyncCallback ACallback, object AState);
		ProcessDescriptor EndStartProcess(IAsyncResult AResult);
		
		/// <summary>
		/// Stops a server process.
		/// </summary>
		/// <param name="AProcessHandle">The handle to the process to be stopped.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginStopProcess(int AProcessHandle, AsyncCallback ACallback, object AState);
		void EndStopProcess(IAsyncResult AResult);
		
		/// <summary>
		/// Begins a transaction.
		/// </summary>
		/// <param name="AProcessHandle">The handle to the process on which the transaction will be started.</param>
		/// <param name="AIsolationLevel">The isolation level of the new transaction.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginBeginTransaction(int AProcessHandle, IsolationLevel AIsolationLevel, AsyncCallback ACallback, object AState);
		void EndBeginTransaction(IAsyncResult AResult);
		
		/// <summary>
		/// Prepares an active transaction to be committed.
		/// </summary>
		/// <param name="AProcessHandle">The handle to the process on which the current transaction will be prepared.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginPrepareTransaction(int AProcessHandle, AsyncCallback ACallback, object AState);
		void EndPrepareTransaction(IAsyncResult AResult);
		
		/// <summary>
		/// Commits an active transaction.
		/// </summary>
		/// <param name="AProcessHandle">The handle to the process on which the current transaction will be committed.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginCommitTransaction(int AProcessHandle, AsyncCallback ACallback, object AState);
		void EndCommitTransaction(IAsyncResult AResult);
		
		/// <summary>
		/// Rolls back an active transaction.
		/// </summary>
		/// <param name="AProcessHandle">The handle to the process on which the current transaction will be rolled back.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginRollbackTransaction(int AProcessHandle, AsyncCallback ACallback, object AState);
		void EndRollbackTransaction(IAsyncResult AResult);
		
		/// <summary>
		/// Gets the number of active transactions.
		/// </summary>
		/// <param name="AProcessHandle">The handle to the process for which the number of active transactions will be returned.</param>
		/// <returns>The number of active transactions.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetTransactionCount(int AProcessHandle, AsyncCallback ACallback, object AState);
		int EndGetTransactionCount(IAsyncResult AResult);

		/// <summary>
		/// Begins an application transaction.
		/// </summary>
		/// <param name="AProcessHandle">The handle to the process that will start the application transaction.</param>
		/// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="AShouldJoin">Whether or not the process initiating the application transaction should immediately join the new transaction.</param>
		/// <param name="AIsInsert">Whether or not the process should join in insert mode.</param>
		/// <returns>The ID of the new application transaction.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBeginApplicationTransaction(int AProcessHandle, ProcessCallInfo ACallInfo, bool AShouldJoin, bool AIsInsert, AsyncCallback ACallback, object AState);
        Guid EndBeginApplicationTransaction(IAsyncResult AResult);
        
        /// <summary>
        /// Prepares an application transaction to be committed.
        /// </summary>
        /// <param name="AProcessHandle">The handle to the process that will perform the prepare.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
        /// <param name="AID">The ID of the application transaction to be prepared.</param>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrepareApplicationTransaction(int AProcessHandle, ProcessCallInfo ACallInfo, Guid AID, AsyncCallback ACallback, object AState);
        void EndPrepareApplicationTransaction(IAsyncResult AResult);

		/// <summary>
		/// Commits an application transaction.
		/// </summary>
		/// <param name="AProcessHandle">The handle to the process that will perform the commit.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="AID">The ID of the application transaction to be committed.</param>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCommitApplicationTransaction(int AProcessHandle, ProcessCallInfo ACallInfo, Guid AID, AsyncCallback ACallback, object AState);
        void EndCommitApplicationTransaction(IAsyncResult AResult);
        
        /// <summary>
        /// Rolls back an application transaction.
        /// </summary>
        /// <param name="AProcessHandle">The handle to the process that will perform the rollback.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
        /// <param name="AID">The ID of the application transaction to be rolled back.</param>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRollbackApplicationTransaction(int AProcessHandle, ProcessCallInfo ACallInfo, Guid AID, AsyncCallback ACallback, object AState);
        void EndRollbackApplicationTransaction(IAsyncResult AResult);
        
        /// <summary>
        /// Gets the ID of the application transaction in which this process is currently participating, if any.
        /// </summary>
        /// <param name="AProcessHandle">The handle to the process.</param>
        /// <returns>The ID of the application transaction.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetApplicationTransactionID(int AProcessHandle, AsyncCallback ACallback, object AState);
        Guid EndGetApplicationTransactionID(IAsyncResult AResult);

		/// <summary>
		/// Joins the process to an application transaction.
		/// </summary>
		/// <param name="AProcessHandle">The handle to the process.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="AID">The ID of the application transaction to join.</param>
		/// <param name="AIsInsert">Whether or not to join in insert mode.</param>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginJoinApplicationTransaction(int AProcessHandle, ProcessCallInfo ACallInfo, Guid AID, bool AIsInsert, AsyncCallback ACallback, object AState);
        void EndJoinApplicationTransaction(IAsyncResult AResult);
        
        /// <summary>
        /// Leaves an application transaction.
        /// </summary>
        /// <param name="AProcessHandle">The handle to the process.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginLeaveApplicationTransaction(int AProcessHandle, ProcessCallInfo ACallInfo, AsyncCallback ACallback, object AState);
		void EndLeaveApplicationTransaction(IAsyncResult AResult);
		
		#endregion
		
		// Plan
		#region Plan
		
		/// <summary>
		/// Prepares a statement for execution.
		/// </summary>
		/// <param name="AProcessHandle">The handle to the process that will prepare the statement.</param>
		/// <param name="ACleanupInfo">A CleanupInfo containing information to be processed prior to the call.</param>
		/// <param name="AStatement">The statement to be prepared.</param>
		/// <param name="AParams">Any parameters to the statement.</param>
		/// <param name="ALocator">A locator describing the source of the statement.</param>
		/// <returns>A PlanDescriptor describing the prepared plan.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginPrepareStatement(int AProcessHandle, ProcessCleanupInfo ACleanupInfo, string AStatement, RemoteParam[] AParams, DebugLocator ALocator, AsyncCallback ACallback, object AState);
		PlanDescriptor EndPrepareStatement(IAsyncResult AResult);
		
		/// <summary>
		/// Executes a prepared plan.
		/// </summary>
		/// <param name="APlanHandle">The handle of the prepared plan to be executed.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="AParams">The parameters to the plan.</param>
		/// <param name="AExecuteTime">The execution time.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginExecutePlan(int APlanHandle, ProcessCallInfo ACallInfo, ref RemoteParamData AParams, out TimeSpan AExecuteTime, AsyncCallback ACallback, object AState);
		void EndExecutePlan(IAsyncResult AResult);

		/// <summary>
		/// Unprepares a prepared plan.
		/// </summary>
		/// <param name="APlanHandle">The handle of the plan to be unprepared.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUnprepareStatement(int APlanHandle, AsyncCallback ACallback, object AState);
		void EndUnprepareStatement(IAsyncResult AResult);
		
		/// <summary>
		/// Prepares an expression for evaluation.
		/// </summary>
		/// <param name="AProcessHandle">The handle to the process.</param>
		/// <param name="ACleanupInfo">A CleanupInfo containing information to be processed prior to the call.</param>
		/// <param name="AExpression">The expression to be prepared.</param>
		/// <param name="AParams">The parameters to the expression.</param>
		/// <param name="ALocator">A debug locator describing the source of the expression.</param>
		/// <returns>A PlanDescriptor describing the prepared plan.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginPrepareExpression(int AProcessHandle, ProcessCleanupInfo ACleanupInfo, string AExpression, RemoteParam[] AParams, DebugLocator ALocator, AsyncCallback ACallback, object AState);
		PlanDescriptor EndPrepareExpression(IAsyncResult AResult);
		
		/// <summary>
		/// Evaluates a prepared plan.
		/// </summary>
		/// <param name="APlanHandle">The handle of the plan to be evaluated.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="AParams">The parameters to the expression.</param>
		/// <param name="AExecuteTime">The execute time.</param>
		/// <returns>The result of evaluating the plan in it's physical representation.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginEvaluatePlan(int APlanHandle, ProcessCallInfo ACallInfo, ref RemoteParamData AParams, out TimeSpan AExecuteTime, AsyncCallback ACallback, object AState);
		byte[] EndEvaluatePlan(IAsyncResult AResult);

		/// <summary>
		/// Opens a cursor from a prepared plan.
		/// </summary>
		/// <param name="APlanHandle">The handle of the plan to be evaluated.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="AParams">The parameters to the expression.</param>
		/// <param name="AExecuteTime">The execution time.</param>
		/// <param name="ABookmarks">A list of bookmarks associated with the inital fetch.</param>
		/// <param name="ACount">The number of rows to be fetched as part of the open.</param>
		/// <param name="AFetchData">A FetchData describing the results of the initial fetch.</param>
		/// <returns>A CursorDescriptor describing the new cursor.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginOpenPlanCursor(int APlanHandle, ProcessCallInfo ACallInfo, ref RemoteParamData AParams, out TimeSpan AExecuteTime, out Guid[] ABookmarks, int ACount, out RemoteFetchData AFetchData, AsyncCallback ACallback, object AState);
		CursorDescriptor EndOpenPlanCursor(IAsyncResult AResult);
		
		/// <summary>
		/// Unprepares a prepared expression plan.
		/// </summary>
		/// <param name="APlanHandle">The handle of the plan to be unprepared.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUnprepareExpression(int APlanHandle, AsyncCallback ACallback, object AState);
		void EndUnprepareExpression(IAsyncResult AResult);
		
		#endregion
		
		// Cursor
		#region Cursor
		
		/// <summary>
		/// Closes an active cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor to be closed.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginCloseCursor(int ACursorHandle, ProcessCallInfo ACallInfo, AsyncCallback ACallback, object AState);
		void EndCloseCursor(IAsyncResult AResult);
		
		/// <summary>
		/// Selects a full row from a cursor.
		/// </summary>
		/// <param name="AHandle">The handle of the cursor from which the row will be selected.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <returns>A RemoteRowBody describing the row in it's physical representation.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginSelect(int ACursorHandle, ProcessCallInfo ACallInfo, AsyncCallback ACallback, object AState);
		RemoteRowBody EndSelect(IAsyncResult AResult);
		
		/// <summary>
		/// Selects a row with a specific set of columns from a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor from which the row will be selected.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="AHeader">A RemoteRowHeader describing the set of columns to be included in the resulting row.</param>
		/// <returns>A RemoteRowBody describing the row in it's physical representation.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginSelectSpecific(int ACursorHandle, ProcessCallInfo ACallInfo, RemoteRowHeader AHeader, AsyncCallback ACallback, object AState);
		RemoteRowBody EndSelectSpecific(IAsyncResult AResult);
		
		/// <summary>
		/// Fetches a number of rows from a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor from which the rows will be fetched.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="ABookmarks">A list of bookmarks associated with the fetched rows.</param>
		/// <param name="ACount">The number of rows to be fetched.</param>
		/// <returns>A RemoteFetchData describing the results of the fetch.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginFetch(int ACursorHandle, ProcessCallInfo ACallInfo, out Guid[] ABookmarks, int ACount, AsyncCallback ACallback, object AState);
		RemoteFetchData EndFetch(IAsyncResult AResult);
		
		/// <summary>
		/// Fetches a number of rows with a specific set of columns from a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor from which the rows will be fetched.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="AHeader">A RemoteRowHeader describing the set of columns to be included in the fetched rows.</param>
		/// <param name="ABookmarks">A list of bookmarks associated with the fetched rows.</param>
		/// <param name="ACount">The number of rows to be fetched.</param>
		/// <returns>A RemoteFetchData describing the results of the fetch.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginFetchSpecific(int ACursorHandle, ProcessCallInfo ACallInfo, RemoteRowHeader AHeader, out Guid[] ABookmarks, int ACount, AsyncCallback ACallback, object AState);
		RemoteFetchData EndFetchSpecific(IAsyncResult AResult);
		
		/// <summary>
		/// Gets the current navigation state of a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor for which the navigation state will be returned.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <returns>A CursorGetFlags describing the navigation state of the cursor.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetFlags(int ACursorHandle, ProcessCallInfo ACallInfo, AsyncCallback ACallback, object AState);
		CursorGetFlags EndGetFlags(IAsyncResult AResult);
		
		/// <summary>
		/// Moves a cursor a number of rows.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor to be navigated.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="ADelta">The number of rows to navigate, forward or backward (negative number)</param>
		/// <returns>A RemoteMoveData describing the results of the move.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginMoveBy(int ACursorHandle, ProcessCallInfo ACallInfo, int ADelta, AsyncCallback ACallback, object AState);
		RemoteMoveData EndMoveBy(IAsyncResult AResult);

		/// <summary>
		/// Positions a cursor before the first row.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor to be navigated.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <returns>A CursorGetFlags describing the navigation state of the cursor.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginFirst(int ACursorHandle, ProcessCallInfo ACallInfo, AsyncCallback ACallback, object AState);
        CursorGetFlags EndFirst(IAsyncResult AResult);

		/// <summary>
		/// Positions a cursor after the last row.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor to be navigated.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <returns>A CursorGetFlags describing the navigation state of the cursor.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginLast(int ACursorHandle, ProcessCallInfo ACallInfo, AsyncCallback ACallback, object AState);
        CursorGetFlags EndLast(IAsyncResult AResult);

		/// <summary>
		/// Resets a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor to be reset.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <returns>A CursorGetFlags describing the navigation state of the cursor.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginReset(int ACursorHandle, ProcessCallInfo ACallInfo, AsyncCallback ACallback, object AState);
        CursorGetFlags EndReset(IAsyncResult AResult);

		/// <summary>
		/// Inserts a row into a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor in which the row will be inserted.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="ARow">The row to be inserted.</param>
		/// <param name="AValueFlags">A value flags array indicating which columns are explicitly specified in the row.</param>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsert(int ACursorHandle, ProcessCallInfo ACallInfo, RemoteRow ARow, BitArray AValueFlags, AsyncCallback ACallback, object AState);
        void EndInsert(IAsyncResult AResult);

		/// <summary>
		/// Updates the current row in a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor in which a row will be updated.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="ARow">The new values for the row.</param>
		/// <param name="AValueFlags">A value flags array indicating which columns are to be updated in the row.</param>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdate(int ACursorHandle, ProcessCallInfo ACallInfo, RemoteRow ARow, BitArray AValueFlags, AsyncCallback ACallback, object AState);
        void EndUpdate(IAsyncResult AResult);
        
        /// <summary>
        /// Deletes the current row in a cursor.
        /// </summary>
        /// <param name="ACursorHandle">The handle of the cursor from which the row will be deleted.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDelete(int ACursorHandle, ProcessCallInfo ACallInfo, AsyncCallback ACallback, object AState);
        void EndDelete(IAsyncResult AResult);

		/// <summary>
		/// Gest a bookmark for the current row in a cursor.
		/// </summary>
        /// <param name="ACursorHandle">The handle of the cursor from which the bookmark will be returned.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <returns>The bookmark for the current row of the cursor.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetBookmark(int ACursorHandle, ProcessCallInfo ACallInfo, AsyncCallback ACallback, object AState);
        Guid EndGetBookmark(IAsyncResult AResult);

		/// <summary>
		/// Positions a cursor on a bookmark.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor to be navigated.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="ABookmark">The bookmark of the row on which the cursor will be positioned.</param>
		/// <param name="AForward">A hint indicating the intended direction of navigation after the positioning call.</param>
		/// <returns>A RemoteGotoData describing the results of the navigation.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGotoBookmark(int ACursorHandle, ProcessCallInfo ACallInfo, Guid ABookmark, bool AForward, AsyncCallback ACallback, object AState);
		RemoteGotoData EndGotoBookmark(IAsyncResult AResult);

		/// <summary>
		/// Compares two bookmarks from a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor from which the bookmarks to be compared were retrieved.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="ABookmark1">The first bookmark to be compared.</param>
		/// <param name="ABookmark2">The second bookmark to be compared.</param>
		/// <returns>0 if the bookmarks are equal, -1 if the first bookmark is less than the second bookmark, and 1 if the first bookmark is greater than the second bookmark.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCompareBookmarks(int ACursorHandle, ProcessCallInfo ACallInfo, Guid ABookmark1, Guid ABookmark2, AsyncCallback ACallback, object AState);
        int EndCompareBookmarks(IAsyncResult AResult);

		/// <summary>
		/// Disposes a bookmark for a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor for which the bookmark is to be disposed.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="ABookmark">The bookmark to be disposed.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginDisposeBookmark(int ACursorHandle, ProcessCallInfo ACallInfo, Guid ABookmark, AsyncCallback ACallback, object AState);
		void EndDisposeBookmark(IAsyncResult AResult);

		/// <summary>
		/// Disposes a list of bookmarks for a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor for which the bookmarks are to be disposed.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="ABookmarks">The list of bookmarks to be disposed.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginDisposeBookmarks(int ACursorHandle, ProcessCallInfo ACallInfo, Guid[] ABookmarks, AsyncCallback ACallback, object AState);
		void EndDisposeBookmarks(IAsyncResult AResult);

		/// <summary>
		/// Gets a string representing the ordering of a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor for which the order is to be returned.</param>
		/// <returns>The order of the cursor as a string.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOrder(int ACursorHandle, AsyncCallback ACallback, object AState);
        string EndGetOrder(IAsyncResult AResult);
        
        /// <summary>
        /// Gets a key for the current position of a cursor.
        /// </summary>
        /// <param name="ACursorHandle">The handle of the cursor from which the key is to be returned.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
        /// <returns>A RemoteRow representing the key in it's physical representation.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetKey(int ACursorHandle, ProcessCallInfo ACallInfo, AsyncCallback ACallback, object AState);
        RemoteRow EndGetKey(IAsyncResult AResult);
        
        /// <summary>
        /// Attempts to position a cursor on a key.
        /// </summary>
        /// <param name="ACursorHandle">The handle of the cursor to be navigated.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
        /// <param name="AKey">The key on which the cursor should be positioned.</param>
        /// <returns>A RemoteGotoData describing the results of the navigation.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginFindKey(int ACursorHandle, ProcessCallInfo ACallInfo, RemoteRow AKey, AsyncCallback ACallback, object AState);
        RemoteGotoData EndFindKey(IAsyncResult AResult);
        
        /// <summary>
        /// Attempts to position a cursor on the nearest matching row.
        /// </summary>
        /// <param name="ACursorHandle">The handle of the cursor to be navigated.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
        /// <param name="AKey">The key on which the should be positioned.</param>
        /// <returns>A CursorGetFlags describing the resulting navigation state of the cursor.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginFindNearest(int ACursorHandle, ProcessCallInfo ACallInfo, RemoteRow AKey, AsyncCallback ACallback, object AState);
        CursorGetFlags EndFindNearest(IAsyncResult AResult);
        
        /// <summary>
        /// Refreshes a cursor.
        /// </summary>
        /// <param name="ACursorHandle">The handle of the cursor to be refreshed.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
        /// <param name="ARow">The row on which the cursor should be positioned after the refresh.</param>
        /// <returns>A RemoteGotoData describing the results of the refresh.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefresh(int ACursorHandle, ProcessCallInfo ACallInfo, RemoteRow ARow, AsyncCallback ACallback, object AState);
        RemoteGotoData EndRefresh(IAsyncResult AResult);

		/// <summary>
		/// Gets the number of rows in a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle of the cursor to be counted.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <returns>The number of rows in the cursor.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRowCount(int ACursorHandle, ProcessCallInfo ACallInfo, AsyncCallback ACallback, object AState);
        int EndGetRowCount(IAsyncResult AResult);

		/// <summary>
		/// Performs default processing for a row in a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle for the cursor that will perform the default processing.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="ARow">The row to be defaulted.</param>
		/// <param name="AColumn">The name of a column that is being defaulted. Use the empty string to indicate that the entire row is being defaulted.</param>
		/// <returns>A RemoteProposeData containing the results of the call.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDefault(int ACursorHandle, ProcessCallInfo ACallInfo, RemoteRowBody ARow, string AColumn, AsyncCallback ACallback, object AState);
        RemoteProposeData EndDefault(IAsyncResult AResult);
        
        /// <summary>
        /// Performs change processing for a row in a cursor.
        /// </summary>
        /// <param name="ACursorHandle">The handle for the cursor that will perform the change processing.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
        /// <param name="AOldRow">The row before the change that triggered the change call.</param>
        /// <param name="ANewRow">The row after the change that triggered the change call.</param>
        /// <param name="AColumn">The name of the column that triggered the change. Use the empty string to indicate that the entire row is being changed.</param>
        /// <returns>A RemoteProposeData containing the results of the call.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginChange(int ACursorHandle, ProcessCallInfo ACallInfo, RemoteRowBody AOldRow, RemoteRowBody ANewRow, string AColumn, AsyncCallback ACallback, object AState);
        RemoteProposeData EndChange(IAsyncResult AResult);

		/// <summary>
		/// Performs validation processing for a row in a cursor.
		/// </summary>
		/// <param name="ACursorHandle">The handle for the cursor that will perform the validation processing.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="AOldRow">The row before the change that triggered the validation call.</param>
		/// <param name="ANewRow">The row after the change that triggered the validation call.</param>
		/// <param name="AColumn">The name of the column that triggered the validation. Use the empty string to indiate that the entire row is being validated.</param>
		/// <returns>A RemoteProposeData containing the results of the call.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginValidate(int ACursorHandle, ProcessCallInfo ACallInfo, RemoteRowBody AOldRow, RemoteRowBody ANewRow, string AColumn, AsyncCallback ACallback, object AState);
        RemoteProposeData EndValidate(IAsyncResult AResult);

		#endregion
		
		// Script
		#region Script
		
		/// <summary>
		/// Prepares a script for execution.
		/// </summary>
		/// <param name="AProcessHandle">The handle of the process on which the script will be prepared.</param>
		/// <param name="AScript">The script to be prepared.</param>
		/// <param name="ALocator">A debug locator describing the source of the script.</param>
		/// <returns>A ScriptDescriptor describing the new script.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginPrepareScript(int AProcessHandle, string AScript, DebugLocator ALocator, AsyncCallback ACallback, object AState);
		ScriptDescriptor EndPrepareScript(IAsyncResult AResult);

		/// <summary>
		/// Unprepares a script for execution.
		/// </summary>
		/// <param name="AScriptHandle">The handle of the script to be unprepared.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUnprepareScript(int AScriptHandle, AsyncCallback ACallback, object AState);
		void EndUnprepareScript(IAsyncResult AResult);

		/// <summary>
		/// Executes a script.
		/// </summary>
		/// <param name="AProcessHandle">The handle of the process on which the execution will be performed.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="AScript">The script to be executed.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginExecuteScript(int AProcessHandle, ProcessCallInfo ACallInfo, string AScript, AsyncCallback ACallback, object AState);
		void EndExecuteScript(IAsyncResult AResult);
		
		/// <summary>
		/// Gets the text of a batch.
		/// </summary>
		/// <param name="ABatchHandle">The handle of the batch for which the text is to be returned.</param>
		/// <returns>The text of the batch as a string.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetBatchText(int ABatchHandle, AsyncCallback ACallback, object AState);
		string EndGetBatchText(IAsyncResult AResult);
		
		/// <summary>
		/// Prepares a batch for execution.
		/// </summary>
		/// <param name="ABatchHandle">The handle of the batch to be prepared.</param>
		/// <param name="AParams">The parameters to the batch.</param>
		/// <returns>A PlanDescriptor describing the prepared plan.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginPrepareBatch(int ABatchHandle, RemoteParam[] AParams, AsyncCallback ACallback, object AState);
		PlanDescriptor EndPrepareBatch(IAsyncResult AResult);
		
		/// <summary>
		/// Unprepares a batch.
		/// </summary>
		/// <param name="APlanHandle">The handle of the plan to be unprepared.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUnprepareBatch(int APlanHandle, AsyncCallback ACallback, object AState);
		void EndUnprepareBatch(IAsyncResult AResult);
		
		/// <summary>
		/// Executes a batch.
		/// </summary>
		/// <param name="ABatchHandle">The handle of the batch to be executed.</param>
        /// <param name="ACallInfo">A CallInfo containing information to be processed prior to the call.</param>
		/// <param name="AParams">The parameters to the batch.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginExecuteBatch(int ABatchHandle, ProcessCallInfo ACallInfo, ref RemoteParamData AParams, AsyncCallback ACallback, object AState);
		void EndExecuteBatch(IAsyncResult AResult);
		
		#endregion
		
		// Catalog Support
		#region Catalog Support
		
		/// <summary>
		/// Gets a script that can be used to create the repository version of a catalog object.
		/// </summary>
		/// <param name="AProcessHandle">The handle of the process to be used for the call.</param>
		/// <param name="AName">The name of the object to be created.</param>
		/// <param name="ACacheTimeStamp">The cache time stamp.</param>
		/// <param name="AClientCacheTimeStamp">The client cache time stamp.</param>
		/// <param name="ACacheChanged">Indicates whether or not the cache changed.</param>
		/// <returns>A D4 script to create the necessary objects.</returns>
		[OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCatalog(int AProcessHandle, string AName, out long ACacheTimeStamp, out long AClientCacheTimeStamp, out bool ACacheChanged, AsyncCallback ACallback, object AState);
        string EndGetCatalog(IAsyncResult AResult);

		/// <summary>
		/// Returns the fully qualified class name for a registered class.
		/// </summary>
		/// <param name="AProcessHandle">The handle of the process to be used for the call.</param>
		/// <param name="AClassName">The registered class name.</param>
		/// <returns>The fully qualified class name as a string.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetClassName(int AProcessHandle, string AClassName, AsyncCallback ACallback, object AState);
		string EndGetClassName(IAsyncResult AResult);

		/// <summary>
		/// Gets the set of files required to support instantiation of a registered class name.
		/// </summary>
		/// <param name="AProcessHandle">The handle of the process to be used for the call.</param>
		/// <param name="AClassName">The name of the registered class that needs to be instantiated.</param>
		/// <returns>A list of ServerFileInfo describing the necessary files.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetFileNames(int AProcessHandle, string AClassName, AsyncCallback ACallback, object AState);
		ServerFileInfo[] EndGetFileNames(IAsyncResult AResult);

		/// <summary>
		/// Gets a file.
		/// </summary>
		/// <param name="AProcessHandle">The handle of the process to be used for the call.</param>
		/// <param name="ALibraryName">The name of the library that contains the file.</param>
		/// <param name="AFileName">The name of the file to be retrieved.</param>
		/// <returns>A byte[] containing the contents of the file.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetFile(int AProcessHandle, string ALibraryName, string AFileName, AsyncCallback ACallback, object AState);
		byte[] EndGetFile(IAsyncResult AResult);
		
		#endregion

		// Stream Support
		#region	Stream Support
		
		/// <summary>
		/// Allocates a stream.
		/// </summary>
		/// <param name="AProcessHandle">The handle of the process on which the stream will be allocated.</param>
		/// <returns>The ID of the new stream.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginAllocateStream(int AProcessHandle, AsyncCallback ACallback, object AState);
		StreamID EndAllocateStream(IAsyncResult AResult);
		
		/// <summary>
		/// References a stream.
		/// </summary>
		/// <param name="AProcessHandle">The handle of the process on which the stream will be referenced.</param>
		/// <param name="AStreamID">The ID of the stream to be referenced.</param>
		/// <returns>The ID of the new stream.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginReferenceStream(int AProcessHandle, StreamID AStreamID, AsyncCallback ACallback, object AState);
		StreamID EndReferenceStream(IAsyncResult AResult);
		
		/// <summary>
		/// Deallocates a stream.
		/// </summary>
		/// <param name="AProcessHandle">The handle of the process on which the stream will be deallocated.</param>
		/// <param name="AStreamID">The ID of the stream to be deallocated.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginDeallocateStream(int AProcessHandle, StreamID AStreamID, AsyncCallback ACallback, object AState);
		void EndDeallocateStream(IAsyncResult AResult);
		
		/// <summary>
		/// Opens a stream.
		/// </summary>
		/// <param name="AProcessHandle">The handle of the process on which the stream will be opened.</param>
		/// <param name="AStreamID">The ID of the stream to be opened.</param>
		/// <param name="ALockMode">The locking to be used to open the stream.</param>
		/// <returns>The handle of the new stream.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginOpenStream(int AProcessHandle, StreamID AStreamID, LockMode ALockMode, AsyncCallback ACallback, object AState);
		int EndOpenStream(IAsyncResult AResult);
		
		/// <summary>
		/// Closes an open stream.
		/// </summary>
		/// <param name="AStreamHandle">The handle of the stream to be closed.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginCloseStream(int AStreamHandle, AsyncCallback ACallback, object AState);
		void EndCloseStream(IAsyncResult AResult);
		
		/// <summary>
		/// Gets the length of a stream.
		/// </summary>
		/// <param name="AStreamHandle">The handle of the stream for which the length will be returned.</param>
		/// <returns>The number of bytes in the stream as a long.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetStreamLength(int AStreamHandle, AsyncCallback ACallback, object AState);
		long EndGetStreamLength(IAsyncResult AResult);
		
		/// <summary>
		/// Sets the length of a stream.
		/// </summary>
		/// <param name="AStreamHandle">The handle of the stream for which the length will be set.</param>
		/// <param name="AValue">The new length of the stream.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginSetStreamLength(int AStreamHandle, long AValue, AsyncCallback ACallback, object AState);
		void EndSetStreamLength(IAsyncResult AResult);
		
		/// <summary>
		/// Gets the current position of a stream.
		/// </summary>
		/// <param name="AStreamHandle">The handle of the stream for which the current position is to be returned.</param>
		/// <returns>The current position of the stream as a long.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetStreamPosition(int AStreamHandle, AsyncCallback ACallback, object AState);
		long EndGetStreamPosition(IAsyncResult AResult);
		
		/// <summary>
		/// Sets the current position of a stream.
		/// </summary>
		/// <param name="AStreamHandle">The handle of the stream for which the current position is to be set.</param>
		/// <param name="APosition">The new position of the stream.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginSetStreamPosition(int AStreamHandle, long APosition, AsyncCallback ACallback, object AState);
		void EndSetStreamPosition(IAsyncResult AResult);
		
		/// <summary>
		/// Flushes a stream.
		/// </summary>
		/// <param name="AStreamHandle">The handle of the stream to be flushed.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginFlushStream(int AStreamHandle, AsyncCallback ACallback, object AState);
		void EndFlushStream(IAsyncResult AResult);

		/// <summary>
		/// Reads from a stream.
		/// </summary>
		/// <param name="AStreamHandle">The handle of the stream to be read.</param>
		/// <param name="ACount">The number of bytes to read.</param>
		/// <returns>A byte[] containing the bytes read.</returns>
		/// <remarks>
		/// Note that the result byte[] may contain less than ACount bytes if the actual number of bytes available to be read in the stream
		/// was less than the requested number.
		/// </remarks>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginReadStream(int AStreamHandle, int ACount, AsyncCallback ACallback, object AState);
		byte[] EndReadStream(IAsyncResult AResult);
		
		/// <summary>
		/// Seeks on a stream.
		/// </summary>
		/// <param name="AStreamHandle">The handle of the stream to be navigated.</param>
		/// <param name="AOffset">The number of bytes to seek.</param>
		/// <param name="AOrigin">The origin of the seek.</param>
		/// <returns>The new position of the stream.</returns>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginSeekStream(int AStreamHandle, long AOffset, SeekOrigin AOrigin, AsyncCallback ACallback, object AState);
		long EndSeekStream(IAsyncResult AResult);

		/// <summary>
		/// Writes to a stream.
		/// </summary>
		/// <param name="AStreamHandle">The handle of the stream to be written.</param>
		/// <param name="AData">A byte[] containing the data to be written.</param>
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginWriteStream(int AStreamHandle, byte[] AData, AsyncCallback ACallback, object AState);
		void EndWriteStream(IAsyncResult AResult);
		
		#endregion
	}
}
