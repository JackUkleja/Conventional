﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Conventional.Conventions.Assemblies
{
    public class MustHaveFilesBeContentConventionSpecification : AssemblyConventionSpecification
    {
        private readonly Regex _fileMatchRegex;
        private readonly string _friendlyDescription;

        private const string EndsWithExtensionPattern = @"\.{0}$";

        public MustHaveFilesBeContentConventionSpecification(string fileExtension)
        {
            _fileMatchRegex = BuildRegExFromFileExtensions(fileExtension);
            _friendlyDescription = fileExtension;
        }

        public MustHaveFilesBeContentConventionSpecification(Regex fileMatchRegex)
        {
            _fileMatchRegex = fileMatchRegex;
            _friendlyDescription = fileMatchRegex.ToString();
        }

        protected override string FailureMessage
        {
            get
            {
                return "All files matching '{0}' within assembly '{1}' must have their build action set to 'Content - Copy if newer'";
            }
        }

        protected override ConventionResult IsSatisfiedByInternal(string assemblyName, XDocument projectDocument)
        {

            var failures = ItemGroupItem.FromProjectDocument(projectDocument)
                .Where(itemGroupItem => itemGroupItem.MatchesPatternAndIsNotContentCopyNewest(_fileMatchRegex))
                .ToArray();

            if (failures.Any())
            {
                var failureText = FailureMessage.FormatWith(_friendlyDescription, assemblyName) +
                                  Environment.NewLine +
                                  string.Join(Environment.NewLine, (IEnumerable<string>)failures.Select(itemGroupItem => "- " + itemGroupItem.ToString()));

                return ConventionResult.NotSatisfied(assemblyName, failureText);
            }

            return ConventionResult.Satisfied(assemblyName);
        }

        private Regex BuildRegExFromFileExtensions(string fileExtension)
        {
            // Note: fileExtension may be *.sql or sql so handle both cases
            var fileExtensionWithoutLeadingPeriodOrWildcard = fileExtension
                .TrimStart('*')
                .TrimStart('.');

            var regExPattern = EndsWithExtensionPattern.FormatWith(fileExtensionWithoutLeadingPeriodOrWildcard);

            return new Regex(regExPattern, RegexOptions.IgnoreCase);
        }
    }
}