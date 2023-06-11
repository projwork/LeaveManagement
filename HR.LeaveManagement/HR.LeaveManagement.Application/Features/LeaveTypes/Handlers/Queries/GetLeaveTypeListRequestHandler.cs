using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Queries;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveTypes.Handlers.Queries
{
    public class GetLeaveTypeListRequestHandler : IRequestHandler<GetLeaveTypeListRequest, List<LeaveTypeDto>>
    {
        private readonly ILeaveTypeRepository _leaveTypeRepository;

        public GetLeaveTypeListRequestHandler(ILeaveTypeRepository leaveTypeRepository)
        {
            _leaveTypeRepository = leaveTypeRepository;
        }

        public async Task<List<LeaveTypeDto>> Handle(GetLeaveTypeListRequest request, CancellationToken cancellationToken)
        {
            var leaveTypes = await _leaveTypeRepository.GetAll();
            var leaveTypeDtoList = new List<LeaveTypeDto>();

            foreach (var leaveType in leaveTypes)
            {
                var leaveTypeDto = new LeaveTypeDto()
                {
                    Id = leaveType.Id,
                    Name = leaveType.Name,
                    DefaultDays = leaveType.DefaultDays
                };
                leaveTypeDtoList.Add(leaveTypeDto);
            }

            return leaveTypeDtoList;
        }
    }
}
