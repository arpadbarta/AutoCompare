using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoCompare.Helpers;
using AutoCompare.Compilation;
using System.Threading.Tasks;

namespace AutoCompare.Configuration
{
    internal class ComparerConfiguration
    {
        protected Dictionary<string, MemberConfiguration> MemberConfigurations { get; }

        public IComparerEngine Engine { get; }
        public bool CompareFields { get; protected set; }
        public HashSet<Type> IgnoredTypes { get; }

        public ComparerConfiguration(IComparerEngine engine)
        {
            Engine = engine;
            IgnoredTypes = new HashSet<Type>();
            MemberConfigurations = new Dictionary<string, MemberConfiguration>();
        }

        public MemberConfiguration GetMemberConfiguration(string memberName)
        {
            return MemberConfigurations.ContainsKey(memberName) ? MemberConfigurations[memberName] : new MemberConfiguration();
        }
    }

    internal class ComparerConfiguration<T> : ComparerConfiguration, IComparerConfiguration<T>, IPrecompile where T : class
    {
        public ComparerConfiguration(IBuilderEngine engine)
            :base(engine)
        {
        }

        public IComparerConfiguration<T> ComparePublicFields()
        {
            CompareFields = true;
            return this;
        }

        public IComparerConfiguration<T> IgnoreMemberType<TMemberType>()
        {
            IgnoredTypes.Add(typeof(TMemberType));
            return this;
        }

        public IComparerConfiguration<T> For<TMember>(Expression<Func<T, TMember>> member, Action<IMemberConfiguration> configuration)
        {
            var memberInfo = ReflectionHelper.GetMemberInfo(member);
            if (MemberConfigurations.ContainsKey(memberInfo.Name))
            {
                throw new Exception($"The member {memberInfo.Name} is already configured.");
            }
            var config = new MemberConfiguration();
            configuration(config);
            MemberConfigurations.Add(memberInfo.Name, config);
            return this;
        }

        private IComparerConfiguration<T> ForEnumerableInternal<TMember>(string memberName, Action<IEnumerableConfiguration<TMember>> configuration) where TMember : class
        {
            if (MemberConfigurations.ContainsKey(memberName))
            {
                throw new Exception($"The member {memberName} is already configured.");
            }
            var config = new EnumerableConfiguration<TMember>();
            configuration(config);
            MemberConfigurations.Add(memberName, config);
            return this;
        }

        public IComparerConfiguration<T> For<TMember>(Expression<Func<T, IEnumerable<TMember>>> member, Action<IEnumerableConfiguration<TMember>> configuration) where TMember : class
        {
            return ForEnumerableInternal(ReflectionHelper.GetMemberInfo(member).Name, configuration);
        }

        public IComparerConfiguration<T> For<TMember>(Expression<Func<T, List<TMember>>> member, Action<IEnumerableConfiguration<TMember>> configuration) where TMember : class
        {
            return ForEnumerableInternal(ReflectionHelper.GetMemberInfo(member).Name, configuration);
        }

        public IComparerConfiguration<T> For<TMember>(Expression<Func<T, IList<TMember>>> member, Action<IEnumerableConfiguration<TMember>> configuration) where TMember : class
        {
            return ForEnumerableInternal(ReflectionHelper.GetMemberInfo(member).Name, configuration);
        }

        public IComparerConfiguration<T> For<TMember>(Expression<Func<T, TMember[]>> member, Action<IEnumerableConfiguration<TMember>> configuration) where TMember : class
        {
            return ForEnumerableInternal(ReflectionHelper.GetMemberInfo(member).Name, configuration);
        }

        public IComparerConfiguration<T> Ignore<TMember>(Expression<Func<T, TMember>> ignoreExpression)
        {
            For(ignoreExpression, x => x.Ignore());
            return this;
        }

        public IPrecompile Compile {
            get
            {
                return this;
            }
        }

        void IPrecompile.Now()
        {
            ((IBuilderEngine)Engine).Compile<T>();
        }

        void IPrecompile.Async()
        {
            new Task(() => Compile.Now()).Start();
        }
    }
}
