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
    public class GetLeaveAllocationListRequestHandler : IRequestHandler<GetLeaveAllocationListRequest, List<LeaveAllocationDto>>
    {
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;

        public GetLeaveAllocationListRequestHandler(ILeaveAllocationRepository leaveAllocationRepository)
        {
            _leaveAllocationRepository = leaveAllocationRepository;
        }

        public async Task<List<LeaveAllocationDto>> Handle(GetLeaveAllocationListRequest request, CancellationToken cancellationToken)
        {
            var leaveAllocations = await _leaveAllocationRepository.GetLeaveAllocationsWithDetails();
            var leaveAllocationDtoList = new List<LeaveAllocationDto>();

            foreach (var leaveAllocation in leaveAllocations)
            {
                var leaveTypeDto = new LeaveAllocationDto()
                {
                    Id = leaveAllocation.Id,
                    NumberOfDays = leaveAllocation.NumberOfDays,
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
                leaveAllocationDtoList.Add(leaveTypeDto);
            }

            return leaveAllocationDtoList;
        }
    }
}
