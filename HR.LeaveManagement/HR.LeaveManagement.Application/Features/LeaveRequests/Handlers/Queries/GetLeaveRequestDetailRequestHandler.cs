using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.DTOs.LeaveRequest;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Queries;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveRequests.Handlers.Queries
{
    public class GetLeaveRequestDetailRequestHandler : IRequestHandler<GetLeaveRequestDetailRequest, LeaveRequestDto>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public GetLeaveRequestDetailRequestHandler(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<LeaveRequestDto> Handle(GetLeaveRequestDetailRequest request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.GetLeaveRequestWithDetails(request.Id);
            var leaveRequestDto = new LeaveRequestDto()
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
                LeaveTypeId = leaveRequest.LeaveTypeId,
                DateRequested = leaveRequest.DateRequested,
                RequestComments = leaveRequest.RequestComments,
                DateActioned = leaveRequest.DateActioned,
                Approved = leaveRequest.Approved,
                Cancelled = leaveRequest.Cancelled
            };

            return leaveRequestDto;
        }
    }
}
