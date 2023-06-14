using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Constants;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.DTOs.LeaveAllocation;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Features.LeaveAllocations.Requests.Queries;
using HR.LeaveManagement.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HR.LeaveManagement.Application.Features.LeaveAllocations.Handlers.Queries
{
    public class GetLeaveAllocationListRequestHandler : IRequestHandler<GetLeaveAllocationListRequest, List<LeaveAllocationDto>>
    {
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public GetLeaveAllocationListRequestHandler(ILeaveAllocationRepository leaveAllocationRepository, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _leaveAllocationRepository = leaveAllocationRepository;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public async Task<List<LeaveAllocationDto>> Handle(GetLeaveAllocationListRequest request, CancellationToken cancellationToken)
        {
            var leaveAllocations = new List<LeaveAllocation>();
            var allocations = new List<LeaveAllocationDto>();

            if (request.IsLoggedInUser)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(
                    q => q.Type == CustomClaimTypes.Uid)?.Value;
                leaveAllocations = await _leaveAllocationRepository.GetLeaveAllocationsWithDetails(userId);

                var employee = await _userService.GetEmployee(userId);

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
                        EmployeeId = employee.Id,
                        LeaveTypeId = leaveAllocation.LeaveTypeId,
                        Period = leaveAllocation.Period
                    };
                    allocations.Add(leaveTypeDto);
                }
                foreach (var alloc in allocations)
                {
                    alloc.Employee = employee;
                }
            }
            else
            {
                leaveAllocations = await _leaveAllocationRepository.GetLeaveAllocationsWithDetails();

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
                    allocations.Add(leaveTypeDto);
                }

                foreach (var req in allocations)
                {
                    req.Employee = await _userService.GetEmployee(req.EmployeeId);
                }
            }

            return allocations;

            var leaveAllocationDtoList = new List<LeaveAllocationDto>();

            

            return leaveAllocationDtoList;
        }
    }
}
