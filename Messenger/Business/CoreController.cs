using System;

namespace Business
{
    public class CoreController
    {
        private static CoreController single_instance = null;
        private static BusinessController businessController = null;

        private CoreController(IStore store)
        {
            businessController = new BusinessController(store);
        }

        public static void Build(IStore store)
        {
            if (single_instance == null)
                single_instance = new CoreController(store);
            else 
                throw new InvalidOperationException("Building more than one instance of the store.");
        }

        public static BusinessController BusinessControllerInstance()
        {
            return businessController;
        }

        public static void Reset()
        {
            businessController = null;
        }
    }
}