﻿using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Core.Common.Tests.Test_Classes
{
    internal class TestClass : ObjectBase
    {
        string _CleanProp = string.Empty;
        string _DirtyProp = string.Empty;
        string _StringProp = string.Empty;
        TestChild _Child = new TestChild();
        TestChild _NotNavigableChild = new TestChild();

        public string CleanProp
        {
            get
            {
                return _CleanProp;
            }

            set
            {
                if(_CleanProp == value)
                {
                    return;
                }
                _CleanProp = value;
                OnPropertyChanged(() => CleanProp, false);
            }
        }

        public string DirtyProp
        {
            get
            {
                return _DirtyProp;
            }

            set
            {
                if(_DirtyProp == value)
                {
                    return;
                }
                _DirtyProp = value;
                OnPropertyChanged(() => DirtyProp, false);
            }
        }

        public string StringProp
        {
            get
            {
                return _StringProp;
            }

            set
            {
                if(_StringProp == value)
                {
                    return;
                }
                _StringProp = value;
                OnPropertyChanged(()=>_StringProp, false);
            }
        }
        public TestChild Child
        {
            get
            {
                return _Child;
            }
        }

        public TestChild NotNavigableChild
        {
            get
            {
                return _NotNavigableChild;
            }

        }
        class TestClassValidator : AbstractValidator<TestClass>
        {
            public TestClassValidator()
            {
                RuleFor(obj => obj.StringProp).NotEmpty();
            }
        }
        protected override IValidator GetValidator()
        {
            return new TestClassValidator();
        }
    }
}
