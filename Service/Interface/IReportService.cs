using DataAccess.Model;
using Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IReportService
    {
        Task<ReportViewModel> GenerateReportAsync(DateTime startDate, DateTime endDate);
    }
}
