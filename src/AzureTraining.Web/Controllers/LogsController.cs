﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AzureTraining.Core;
using AzureTraining.Web.Models;
using PagedList;

namespace AzureTraining.Web.Controllers
{
    [Authorize]
    public partial class LogsController : Controller
    {
        const int pageSize = 4;

        private ILogger _logger;

        public LogsController(ILogger logger)
        {
            this._logger = logger; 
        }

        public virtual ActionResult Index(int page = 1, string documentName = null, bool orderByDate = false)
        {
            var login = User.Identity.Name;
            var logs = documentName == null ? _logger.GetLogs(login) : _logger.GetLogsForDocument(login, documentName);
            if (orderByDate)
                logs = logs.OrderBy(x => x.Date);

            ViewBag.page = page;
            ViewBag.documentName = documentName;
            ViewBag.orderByDate = orderByDate;

            return View(logs.ToPagedList(page, pageSize));
        }
    }
}
