using System;
using System.Collections.Generic;
#if !DISABLE_PLAYFABCLIENT_API
using PlayFab.ClientModels;
#endif

namespace PlayFab.UUnit
{
    public static class UUnitIncrementalTestRunner
    {
        public static bool SuiteFinished { get; private set; }
        public static bool AllTestsPassed { get; private set; }
        public static string Summary { get; private set; }
        private static UUnitTestSuite _suite;
        private static bool _postResultsToCloudscript;
#if !DISABLE_PLAYFABCLIENT_API
        private static Action<PlayFabResult<ExecuteCloudScriptResult>> _onComplete;
#endif

        public static void Start(bool postResultsToCloudscript = true, string filter = null, Dictionary<string, string> testInputs = null
#if !DISABLE_PLAYFABCLIENT_API
            , Action<PlayFabResult<ExecuteCloudScriptResult>> onComplete = null
#endif
        )
        {
            // Fall back on hard coded testTitleData if necessary (Put your own data here)
            if (testInputs == null)
                testInputs = new Dictionary<string, string> { { "titleId", "6195" }, { "userEmail", "paul@playfab.com" } };
#if !DISABLE_PLAYFABCLIENT_API
            PlayFabApiTest.SetTitleInfo(testInputs);
#endif

            SuiteFinished = false;
            AllTestsPassed = false;
            _postResultsToCloudscript = postResultsToCloudscript;
            _suite = new UUnitTestSuite();
            _suite.FindAndAddAllTestCases(typeof(UUnitTestCase), filter);
#if !DISABLE_PLAYFABCLIENT_API
            _onComplete = onComplete;
#endif
        }

        public static string Tick()
        {
            if (SuiteFinished)
                return Summary;

            SuiteFinished = _suite.TickTestSuite();
            Summary = _suite.GenerateSummary();
            AllTestsPassed = _suite.AllTestsPassed();

            if (SuiteFinished)
                OnSuiteFinish();

            return Summary;
        }

        private static void OnSuiteFinish()
        {
            if (_postResultsToCloudscript)
                PostTestResultsToCloudScript(_suite.GetInternalReport());
        }

        private static void PostTestResultsToCloudScript(TestSuiteReport testReport)
        {
#if !DISABLE_PLAYFABCLIENT_API
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "SaveTestData",
                FunctionParameter = new Dictionary<string, object> { { "customId", PlayFabSettings.BuildIdentifier }, { "testReport", new[] { testReport } } },
                GeneratePlayStreamEvent = true
            };
            var saveTask = PlayFabClientAPI.ExecuteCloudScriptAsync(request);
            saveTask.ContinueWith(task =>
            {
                if (_onComplete != null)
                    _onComplete(task.Result);
            }
            );
#endif
        }
    }
}
