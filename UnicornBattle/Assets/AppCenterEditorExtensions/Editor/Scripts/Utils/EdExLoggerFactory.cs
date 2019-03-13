
//Factory.
namespace AppCenterEditor
{
    class EdExLogger
    {
        private static IEdExLogger _instance;

        public static IEdExLogger LoggerInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalLogger();
                }
                return _instance;
            }
        }

        private EdExLogger()
        {
        }        
    }
}
