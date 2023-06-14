using System;
using System.Collections.Generic;
using System.Text;
using HR.LeaveManagement.Application.DTOs.LeaveAllocation;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveAllocations.Requests.Queries
{
    public class GetLeaveAllocationListRequest : IRequest<List<LeaveAllocationDto>>
    {
        public bool IsLoggedInUser { get; set; }
    }
}
