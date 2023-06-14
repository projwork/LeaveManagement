using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.DTOs.LeaveRequest.Validators;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Commands;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveRequests.Handlers.Commands
{
    public class UpdateLeaveRequestCommandHandler : IRequestHandler<UpdateLeaveRequestCommand, Unit>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;

        public UpdateLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository, ILeaveTypeRepository leaveTypeRepository, ILeaveAllocationRepository leaveAllocationRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _leaveAllocationRepository = leaveAllocationRepository;
        }

        public async Task<Unit> Handle(UpdateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var leaveRequest = await _leaveRequestRepository.Get(request.Id);

            if (leaveRequest is null)
                throw new NotFoundException(nameof(leaveRequest), request.Id);

            if (request.LeaveRequestDto != null)
            {
                var validator = new UpdateLeaveRequestDtoValidator(_leaveTypeRepository);
                var validationResult = await validator.ValidateAsync(request.LeaveRequestDto);
                if (validationResult.IsValid == false)
                    throw new ValidationException(validationResult);

                leaveRequest.Id = request.LeaveRequestDto.Id;
                leaveRequest.LeaveTypeId = request.LeaveRequestDto.LeaveTypeId;
                leaveRequest.StartDate = request.LeaveRequestDto.StartDate;
                leaveRequest.EndDate = request.LeaveRequestDto.EndDate;
                leaveRequest.Cancelled = request.LeaveRequestDto.Cancelled;
                leaveRequest.RequestComments = request.LeaveRequestDto.RequestComments;

                await _leaveRequestRepository.Update(leaveRequest);
            }
            else if (request.ChangeLeaveRequestApprovalDto != null)
            {
                await _leaveRequestRepository.ChangeApprovalStatus(leaveRequest,
                    request.ChangeLeaveRequestApprovalDto.Approved);
                if (request.ChangeLeaveRequestApprovalDto.Approved)
                {
                    var allocation = await _leaveAllocationRepository.GetUserAllocations(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);
                    int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;

                    allocation.NumberOfDays -= daysRequested;

                    await _leaveAllocationRepository.Update(allocation);
                }
            }

            return Unit.Value;
        }
    }
}
