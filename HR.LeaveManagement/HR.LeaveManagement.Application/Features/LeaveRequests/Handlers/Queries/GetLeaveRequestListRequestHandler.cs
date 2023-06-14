using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Constants;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.DTOs.LeaveAllocation;
using HR.LeaveManagement.Application.DTOs.LeaveRequest;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Queries;
using HR.LeaveManagement.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HR.LeaveManagement.Application.Features.LeaveRequests.Handlers.Queries
{
    public class GetLeaveRequestListRequestHandler : IRequestHandler<GetLeaveRequestListRequest, List<LeaveRequestListDto>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public GetLeaveRequestListRequestHandler(ILeaveRequestRepository leaveRequestRepository, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public async Task<List<LeaveRequestListDto>> Handle(GetLeaveRequestListRequest request, CancellationToken cancellationToken)
        {
            var leaveRequests = new List<LeaveRequest>();
            var requests = new List<LeaveRequestListDto>();

            if (request.IsLoggedInUser)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(
                    q => q.Type == CustomClaimTypes.Uid)?.Value;
                leaveRequests = await _leaveRequestRepository.GetLeaveRequestsWithDetails(userId);

                var employee = await _userService.GetEmployee(userId);

                foreach (var leaveRequest in leaveRequests)
                {
                    var leaveRequestDto = new LeaveRequestListDto()
                    {
                        Id = leaveRequest.Id,
                        StartDate = leaveRequest.StartDate,
                        EndDate = leaveRequest.EndDate,
                        RequestingEmployeeId = employee.Id,
                        LeaveType = leaveRequest.LeaveType == null
                            ? null
                            : new LeaveTypeDto()
                            {
                                Id = leaveRequest.LeaveType.Id,
                                Name = leaveRequest.LeaveType.Name,
                                DefaultDays = leaveRequest.LeaveType.DefaultDays
                            },
                        DateRequested = leaveRequest.DateCreated,
                        Approved = leaveRequest.Approved
                    };
                    requests.Add(leaveRequestDto);
                }

                foreach (var req in requests)
                {
                    req.Employee = employee;
                }
            }
            else
            {
                leaveRequests = await _leaveRequestRepository.GetLeaveRequestsWithDetails();

                foreach (var leaveRequest in leaveRequests)
                {
                    var leaveRequestDto = new LeaveRequestListDto()
                    {
                        Id = leaveRequest.Id,
                        StartDate = leaveRequest.StartDate,
                        EndDate = leaveRequest.EndDate,
                        RequestingEmployeeId = leaveRequest.RequestingEmployeeId,
                        LeaveType = leaveRequest.LeaveType == null
                            ? null
                            : new LeaveTypeDto()
                            {
                                Id = leaveRequest.LeaveType.Id,
                                Name = leaveRequest.LeaveType.Name,
                                DefaultDays = leaveRequest.LeaveType.DefaultDays
                            },
                        DateRequested = leaveRequest.DateCreated,
                        Approved = leaveRequest.Approved
                    };
                    requests.Add(leaveRequestDto);
                }

                foreach (var req in requests)
                {
                    req.Employee = await _userService.GetEmployee(req.RequestingEmployeeId);
                }
            }
            return requests;
        }
    }
}
