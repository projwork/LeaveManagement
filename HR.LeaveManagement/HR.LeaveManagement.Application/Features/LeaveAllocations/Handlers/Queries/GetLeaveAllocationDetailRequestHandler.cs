using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.DTOs.LeaveAllocation;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Features.LeaveAllocations.Requests.Queries;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveAllocations.Handlers.Queries
{
    public class GetLeaveAllocationDetailRequestHandler : IRequestHandler<GetLeaveAllocationDetailRequest, LeaveAllocationDto>
    {
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;

        public GetLeaveAllocationDetailRequestHandler(ILeaveAllocationRepository leaveAllocationRepository)
        {
            _leaveAllocationRepository = leaveAllocationRepository;
        }
        public async Task<LeaveAllocationDto> Handle(GetLeaveAllocationDetailRequest request, CancellationToken cancellationToken)
        {
            var leaveAllocation = await _leaveAllocationRepository.GetLeaveAllocationWithDetails(request.Id);
            var leaveAllocationDto = new LeaveAllocationDto()
            {
                Id = leaveAllocation.Id,
                LeaveType = leaveAllocation.LeaveType == null
                    ? null
                    : new LeaveTypeDto()
                    {
                        Id = leaveAllocation.LeaveType.Id,
                        Name = leaveAllocation.LeaveType.Name,
                        DefaultDays = leaveAllocation.LeaveType.DefaultDays
                    },
                EmployeeId = leaveAllocation.EmployeeId,
                LeaveTypeId = leaveAllocation.LeaveTypeId,
                Period = leaveAllocation.Period
            };

            return leaveAllocationDto;
        }
    }
}
