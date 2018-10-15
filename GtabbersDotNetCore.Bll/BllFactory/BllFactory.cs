using System;
using System.Collections.Generic;
using System.Text;
using GtabbersDotNetCore.Bll.BllRepositories;
using GtabbersDotNetCore.Bll.BllRepositories.Robots;

namespace GtabbersDotNetCore.Bll.BllFactory
{

    public interface IBllFactory
    {
        IRobotsFactory RobotsFactory { get; }
        IActionsFactory ActionsFactory { get; }
    }

    public class BllFactory : IBllFactory
    {
        public BllFactory()
        {
        }

        private IRobotsFactory _robotsFactory;
        public IRobotsFactory RobotsFactory => _robotsFactory ?? (_robotsFactory = new RobotsFactory());

        private IActionsFactory _actionsFactory;
        public IActionsFactory ActionsFactory => _actionsFactory ?? (_actionsFactory = new ActionsFactory(RobotsFactory));
    }
}
