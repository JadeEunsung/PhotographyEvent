﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PhotographyEvent.Events
{
    
    // This page shows previous events list
    public partial class PreviousEvents : System.Web.UI.Page
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnRetrieve_Click(object sender, EventArgs e)
        {
            BindData();
        }

        // selects data and show them on the table
        private void BindData()
        {
            string select = @"select ROW_NUMBER() over (order by a.eventid) as RowNo, a.EventId, a.EventName,
	                            (a.StartDate + ' ~ ' + a.EndDate) as EventDate, c.FirstName as WinnerName,
	                            COUNT(b.UserId) as UserCount
                            from Events as A
                            left outer join EventUserPhotos as B on a.EventId = b.EventId
                            left outer join Users as c on a.Winner = c.UserId
                            where a.IsClosed = 1
                            group by a.EventId, a.EventName, a.StartDate, a.EndDate, c.FirstName";

            using (System.Data.DataSet ds = Libs.DbHandler.getResultAsDataSet(select, null))
            {
                gvPrevEvents.DataSource = ds;
                gvPrevEvents.DataBind();
            }
        }
    }
}