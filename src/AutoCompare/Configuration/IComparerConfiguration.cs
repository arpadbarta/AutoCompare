﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AutoCompare.Configuration
{
    /// <summary>
    /// Configures how the engine handles a specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IComparerConfiguration<T>
    {
        /// <summary>
        /// By default, only public properties are compared. Call this to enable comparing public fields for the specified type.
        /// </summary>
        /// <returns></returns>
        IComparerConfiguration<T> ComparePublicFields();

        /// <summary>
        /// By default, all types are compared. Call this to disable comparison for specific types.
        /// </summary>
        /// <typeparam name="TMemberType">Type which should be ignored.</typeparam>
        IComparerConfiguration<T> IgnoreMemberType<TMemberType>();

        /// <summary>
        /// Configures how the engine handles the specified member
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="member"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IComparerConfiguration<T> For<TMember>(Expression<Func<T, TMember>> member, Action<IMemberConfiguration> configuration);

        /// <summary>
        /// Configures how the engine handles the specified IEnumerable member
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="member"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IComparerConfiguration<T> For<TMember>(Expression<Func<T, IEnumerable<TMember>>> member, Action<IEnumerableConfiguration<TMember>> configuration) where TMember : class;

        /// <summary>
        /// Configures how the engine handles the specified List member
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="member"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IComparerConfiguration<T> For<TMember>(Expression<Func<T, List<TMember>>> member, Action<IEnumerableConfiguration<TMember>> configuration) where TMember : class;

        /// <summary>
        /// Configures how the engine handles the specified IList member
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="member"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IComparerConfiguration<T> For<TMember>(Expression<Func<T, IList<TMember>>> member, Action<IEnumerableConfiguration<TMember>> configuration) where TMember : class;

        /// <summary>
        /// Configures how the engine handles the specified array member
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="member"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IComparerConfiguration<T> For<TMember>(Expression<Func<T, TMember[]>> member, Action<IEnumerableConfiguration<TMember>> configuration) where TMember : class;

        /// <summary>
        /// Ignores a member. A shortcut for 
        /// .For(x => x.Member, x => x.Ignore())
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="member">The member to ignore</param>
        /// <returns>self</returns>
        IComparerConfiguration<T> Ignore<TMember>(Expression<Func<T, TMember>> member);

        /// <summary>
        /// Instructs the IComparerEngine how to precompile this comparer
        /// </summary>
        IPrecompile Compile { get; }
    }
}
