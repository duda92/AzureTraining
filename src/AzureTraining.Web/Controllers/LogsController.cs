﻿using System;
using System.Linq;
using System.Web.Mvc;
using AzureTraining.Core;

namespace AzureTraining.Web.Controllers
{
    [Authorize]
    public partial class LogsController : Controller
    {
        private IAzureLogger _logger;

        public LogsController(IAzureLogger logger)
        {
            this._logger = logger; 
        }

        public virtual ActionResult Index()
        {
            var login = User.Identity.Name;
            var logs = _logger.GetLogs(login);
            return View(logs);
        }

    }
}
