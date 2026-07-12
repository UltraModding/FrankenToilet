using System;
using JetBrains.Annotations;

namespace FrankenToilet.mercy;

/// <summary>
/// Marks a feature eligible for the random feature pool activated every 10-20 seconds.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class MercyFeatureAttribute : Attribute;