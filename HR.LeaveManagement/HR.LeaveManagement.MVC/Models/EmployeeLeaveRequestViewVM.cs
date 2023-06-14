using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR.LeaveManagement.MVC.Models
{
    public class EmployeeLeaveRequestViewVM
    {
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
        public List<LeaveRequestVM> LeaveRequests { get; set; }
    }
}
