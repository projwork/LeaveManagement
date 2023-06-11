using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.DTOs.LeaveAllocation;
using HR.LeaveManagement.Application.DTOs.LeaveRequest;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Queries;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveRequests.Handlers.Queries
{
    public class GetLeaveRequestListRequestHandler : IRequestHandler<GetLeaveRequestListRequest, List<LeaveRequestListDto>>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public GetLeaveRequestListRequestHandler(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<List<LeaveRequestListDto>> Handle(GetLeaveRequestListRequest request, CancellationToken cancellationToken)
        {
            var leaveRequests = await _leaveRequestRepository.GetLeaveRequestsWithDetails();
            var leaveRequestDtoList = new List<LeaveRequestListDto>();

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
                    DateRequested = leaveRequest.DateRequested,
                    Approved = leaveRequest.Approved
                };
                leaveRequestDtoList.Add(leaveRequestDto);
            }

            return leaveRequestDtoList;
        }
    }
}
