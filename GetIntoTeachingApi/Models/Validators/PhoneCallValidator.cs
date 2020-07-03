﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class PhoneCallValidator : AbstractValidator<PhoneCall>
    {
        private readonly IStore _store;

        public PhoneCallValidator(IStore store)
        {
            _store = store;

            RuleFor(phoneCall => phoneCall.ScheduledAt).GreaterThan(candidate => DateTime.Now);

            RuleFor(candidate => candidate.ChannelId)
                .Must(id => ChannelIds().Contains(id.ToString()))
                .WithMessage("Must be a valid candidate channel.");
            RuleFor(phoneCall => phoneCall.DestinationId)
                .Must(id => DestinationIds().Contains(id.ToString()))
                .WithMessage("Must be a valid phone call destination.");
        }

        private IEnumerable<string> ChannelIds()
        {
            return _store.GetPickListItems("phonecall", "dfe_channelcreation").Select(channel => channel.Id);
        }

        private IEnumerable<string> DestinationIds()
        {
            return _store.GetPickListItems("phonecall", "dfe_destination").Select(channel => channel.Id);
        }
    }
}