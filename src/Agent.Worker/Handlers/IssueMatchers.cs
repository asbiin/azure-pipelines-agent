using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Services.Agent.Util;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.Agent.Worker.Handlers
{
    public sealed class MatcherChangedEventArgs : EventArgs
    {
    }

    [DataContract]
    public sealed class IssueMatchers
    {
        [DataMember(Name = "problemMatcher")]
        private List<IssueMatcher> _matchers;

        public List<IssueMatcher> Matchers
        {
            get
            {
                if (_matchers == null)
                {
                    _matchers = new List<IssueMatcher>();
                }

                return _matchers;
            }
        }

        public void Validate()
        {
            // todo
        }
    }

    [DataContract]
    public sealed class IssueMatcher
    {
        [DataMember(Name = "owner")]
        private string _owner;

        [DataMember(Name = "pattern")]
        private IssuePattern[] _patterns;

        private IssueMatch[] _state;

        [JsonConstructor]
        public IssueMatcher()
        {
        }

        public IssueMatcher(IssueMatcher copy, Timespan timeout)
        {
            _owner = copy._owner;

            if (copy._patterns?.Length > 0)
            {
                _patterns = copy.Patterns.Select(x => new IssuePattern(x , timeout)).ToArray();
            }
        }

        public string Owner
        {
            get
            {
                if (_owner == null)
                {
                    _owner = String.Empty;
                }

                return _owner;
            }
        }

        public IssuePattern[] Patterns
        {
            get
            {
                if (_patterns == null)
                {
                    _patterns = new IssuePattern[0];
                }

                return _patterns;
            }
        }

        public IssueMatch Match(string line)
        {
            if (_patterns.Length == 1)
            {
            }
            else
            {
                if (_state == null)
                {
                    _state = new IssueMatch[_patterns.Length - 1];
                }

                for (int i = _patterns.Length - 1; i >= 0; i--)
                {
                    var runningMatch = i > 0 ? _state[i - 1] : null;

                    var match = _patterns[i].Match(line, runningMatch);
                }
            }
        }

        public void Reset()
        {
            _matches = null;
        }

        public void Validate()
        {
            // todo: only last pattern may contain "loop=true"
            // todo: pattern may not contain "loop=true" when it is the only pattern
            // todo: only the last pattern may define message
            // todo: the same property may not be defined on more than one pattern
            // todo: the last pattern must define message
            // todo: validate at least one pattern
        }
    }

    [DataContract]
    public sealed class IssuePattern
    {
        private static readonly _options = RegexOptions.CultureInvariant | RegexOptions.ECMAScript | RegexOptions.IgnoreCase;

        [DataMember(Name = "regexp")]
        private string _pattern;

        private Regex _regex;

        private Timespan? _timeout;

        [JsonConstructor]
        public IssuePattern()
        {
        }

        public IssuePattern(IssuePattern copy, Timespan timeout)
        {
            _pattern = copy._pattern;
            File = copy.File;
            Line = copy.Line;
            Column = copy.Column;
            Severity = copy.Severity;
            Code = copy.Code;
            Message = copy.Message;
            FromPath = copy.FromPath;
            _timespan = timeout;
        }

        [DataMember(Name = "file")]
        public int? File { get; set; }

        [DataMember(Name = "line")]
        public int? Line { get; set; }

        [DataMember(Name = "column")]
        public int? Column { get; set; }

        [DataMember(Name = "severity")]
        public int? Severity { get; set; }

        [DataMember(Name = "code")]
        public int? Code { get; set; }

        [DataMember(Name = "message")]
        public int? Message { get; set; }

        [DataMember(Name = "fromPath")]
        public int? FromPath { get; set; }

        [DataMember(Name = "loop")]
        public bool Loop { get; set; }

        public Regex Regex
        {
            get
            {
                if (_regex == null)
                {
                    _regex = new Regex(_pattern ?? String.Empty, _options, _timeout ?? TimeSpan.FromSeconds(1));
                }

                return _regex;
            }
        }
    }

    public sealed class IssueMatch
    {
        public string File { get; set; }

        public string Line { get; set; }

        public string Column { get; set; }

        public string Severity { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public string FromPath { get; set; }
    }
}
