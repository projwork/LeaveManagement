﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveTypes.Handlers.Commands;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Commands;
using HR.LeaveManagement.Application.Responses;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.LeaveTypes.Commands
{
    public class CreateLeaveTypeCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly CreateLeaveTypeDto _leaveTypeDto;
        private readonly CreateLeaveTypeCommandHandler _handler;

        public CreateLeaveTypeCommandHandlerTests()
        {
            _mockUow = MockUnitOfWork.GetUnitOfWork();

            _handler = new CreateLeaveTypeCommandHandler(_mockUow.Object);

            _leaveTypeDto = new CreateLeaveTypeDto
            {
                DefaultDays = 15,
                Name = "Test DTO"
            };
        }

        [Fact]
        public async Task Valid_LeaveType_Added()
        {
            var result = await _handler.Handle(new CreateLeaveTypeCommand() { LeaveTypeDto = _leaveTypeDto }, CancellationToken.None);

            var leaveTypes = await _mockUow.Object.LeaveTypeRepository.GetAll();

            result.ShouldBeOfType<BaseCommandResponse>();

            leaveTypes.Count.ShouldBe(4);
        }

        [Fact]
        public async Task InValid_LeaveType_Added()
        {
            _leaveTypeDto.DefaultDays = -1;

            //ValidationException ex = await Should.ThrowAsync<ValidationException>
            //    (async () =>
            //       await _handler.Handle(new CreateLeaveTypeCommand() { LeaveTypeDto = _leaveTypeDto }, CancellationToken.None)
            //    );

            var result = await _handler.Handle(new CreateLeaveTypeCommand() { LeaveTypeDto = _leaveTypeDto }, CancellationToken.None);

            var leaveTypes = await _mockUow.Object.LeaveTypeRepository.GetAll();

            leaveTypes.Count.ShouldBe(3);

            //ex.ShouldNotBeNull();

            result.ShouldBeOfType<BaseCommandResponse>();

        }
    }
}
