﻿using System;
using Microsoft.CodeAnalysis;
using Xunit;
using Xunit.Analyzers;
using Verify = CSharpVerifier<Xunit.Analyzers.AssertStringEqualityCheckShouldNotUseBoolCheck>;

public class AssertStringEqualityCheckShouldNotUseBoolCheckTest
{
	public static TheoryData<string> Methods = new()
	{
		Constants.Asserts.True,
		Constants.Asserts.False,
	};
	public static TheoryData<StringComparison> SupportedStringComparisons = new()
	{
		StringComparison.Ordinal,
		StringComparison.OrdinalIgnoreCase,
	};
	public static TheoryData<StringComparison> UnsupportedStringComparisons = new()
	{
		StringComparison.CurrentCulture,
		StringComparison.CurrentCultureIgnoreCase,
		StringComparison.InvariantCulture,
		StringComparison.InvariantCultureIgnoreCase,
	};
	public static TheoryData<StringComparison> AllStringComparisons = new()
	{
		StringComparison.Ordinal,
		StringComparison.OrdinalIgnoreCase,
		StringComparison.CurrentCulture,
		StringComparison.CurrentCultureIgnoreCase,
		StringComparison.InvariantCulture,
		StringComparison.InvariantCultureIgnoreCase,
	};

	[Theory]
	[MemberData(nameof(Methods))]
	public async void FindsWarning_ForInstanceEqualsCheck(string method)
	{
		var source = $@"
class TestClass {{
    void TestMethod() {{
        Xunit.Assert.{method}(""abc"".Equals(""a""));
    }}
}}";
		var expected =
			Verify
				.Diagnostic()
				.WithSpan(4, 9, 4, 41 + method.Length)
				.WithSeverity(DiagnosticSeverity.Warning)
				.WithArguments($"Assert.{method}()");

		await Verify.VerifyAnalyzerAsync(source, expected);
	}

	[Theory]
	[MemberData(nameof(SupportedStringComparisons))]
	public async void FindsWarning_ForTrueInstanceEqualsCheck_WithSupportedStringComparison(StringComparison comparison)
	{
		var source = $@"
class TestClass {{
    void TestMethod() {{
        Xunit.Assert.True(""abc"".Equals(""a"", System.StringComparison.{comparison}));
    }}
}}";
		var expected =
			Verify
				.Diagnostic()
				.WithSpan(4, 9, 4, 71 + comparison.ToString().Length)
				.WithSeverity(DiagnosticSeverity.Warning)
				.WithArguments($"Assert.True()");

		await Verify.VerifyAnalyzerAsync(source, expected);
	}

	[Theory]
	[MemberData(nameof(UnsupportedStringComparisons))]
	public async void DoesNotFindWarning_ForTrueInstanceEqualsCheck_WithUnsupportedStringComparison(StringComparison comparison)
	{
		var source = $@"
class TestClass {{
    void TestMethod() {{
        Xunit.Assert.True(""abc"".Equals(""a"", System.StringComparison.{comparison}));
    }}
}}";

		await Verify.VerifyAnalyzerAsync(source);
	}

	[Theory]
	[MemberData(nameof(AllStringComparisons))]
	public async void DoesNotFindWarning_ForFalseInstanceEqualsCheck_WithStringComparison(StringComparison comparison)
	{
		var source = $@"
class TestClass {{
    void TestMethod() {{
        Xunit.Assert.False(""abc"".Equals(""a"", System.StringComparison.{comparison}));
    }}
}}";

		await Verify.VerifyAnalyzerAsync(source);
	}

	[Theory]
	[MemberData(nameof(Methods))]
	public async void FindsWarning_ForStaticEqualsCheck(string method)
	{
		var source = $@"
class TestClass {{
    void TestMethod() {{
        Xunit.Assert.{method}(System.String.Equals(""abc"", ""a""));
    }}
}}";
		var expected =
			Verify
				.Diagnostic()
				.WithSpan(4, 9, 4, 56 + method.Length)
				.WithSeverity(DiagnosticSeverity.Warning)
				.WithArguments($"Assert.{method}()");

		await Verify.VerifyAnalyzerAsync(source, expected);
	}

	[Theory]
	[MemberData(nameof(SupportedStringComparisons))]
	public async void FindsWarning_ForTrueStaticEqualsCheck_WithSupportedStringComparison(StringComparison comparison)
	{
		var source = $@"
class TestClass {{
    void TestMethod() {{
        Xunit.Assert.True(System.String.Equals(""abc"", ""a"", System.StringComparison.{comparison}));
    }}
}}";
		var expected =
			Verify
				.Diagnostic()
				.WithSpan(4, 9, 4, 86 + comparison.ToString().Length)
				.WithSeverity(DiagnosticSeverity.Warning)
				.WithArguments($"Assert.True()");

		await Verify.VerifyAnalyzerAsync(source, expected);
	}

	[Theory]
	[MemberData(nameof(UnsupportedStringComparisons))]
	public async void DoesNotFindWarning_ForTrueStaticEqualsCheck_WithUnsupportedStringComparison(StringComparison comparison)
	{
		var source = $@"
class TestClass {{
    void TestMethod() {{
        Xunit.Assert.True(System.String.Equals(""abc"", ""a"", System.StringComparison.{comparison}));
    }}
}}";

		await Verify.VerifyAnalyzerAsync(source);
	}

	[Theory]
	[MemberData(nameof(AllStringComparisons))]
	public async void DoesNotFindWarning_ForFalseStaticEqualsCheck_WithStringComparison(StringComparison comparison)
	{
		var source = $@"
class TestClass {{
    void TestMethod() {{
        Xunit.Assert.False(System.String.Equals(""abc"", ""a"", System.StringComparison.{comparison}));
    }}
}}";

		await Verify.VerifyAnalyzerAsync(source);
	}
}