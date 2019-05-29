using Framework.Manager;
using Framework.Template;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Framework.Locator
{
    public class ManagerLocator : LocatorBase
    {
        Dictionary<string, IFake<BaseItem>> managerDictionary;

        public ManagerLocator()
        {
            managerDictionary = new Dictionary<string, IFake<BaseItem>>();
        }

        public IFake<BaseItem> GetManager(string managerID)
        {
            if (IsManagerExist(managerID))
                return managerDictionary[managerID];

            return null;
        }

        public void AddManager(IFake<BaseItem> newManager, string managerID)
        {
            if (!IsManagerExist(managerID))
                managerDictionary.Add(managerID, newManager);
        }

        public void RemoveManager(string managerID)
        {
            if (IsManagerExist(managerID))
                managerDictionary.Remove(managerID);
        }

        public bool IsManagerExist(string managerID)
        {
            return managerDictionary.ContainsKey(managerID);
        }

        public Type GetTypeOfManager(string managerID)
        {
            if (IsManagerExist(managerID))
                return managerDictionary[managerID].GetType();

            return null;
        }

        public ObservableCollection<IFake<BaseItem>> GetManagersByType(Type managerType)
        {
            return new ObservableCollection<IFake<BaseItem>>(managerDictionary.Where(x => (x.Value.GetType() == managerType)).Select(y => y.Value).ToList());
        }
    }
}
