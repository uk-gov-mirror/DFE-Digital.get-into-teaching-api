﻿using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidatePastTeachingPositionValidator : AbstractValidator<CandidatePastTeachingPosition>
    {
        public CandidatePastTeachingPositionValidator(IStore store)
        {
            RuleFor(position => position.EducationPhaseId)
                .SetValidator(new PickListItemIdValidator("dfe_candidatepastteachingposition", "dfe_educationphase", store))
                .Unless(position => position.EducationPhaseId == null);
            RuleFor(position => position.SubjectTaughtId)
                .SetValidator(new LookupItemIdValidator("dfe_teachingsubjectlist", store))
                .Unless(position => position.SubjectTaughtId == null);
        }
    }
}