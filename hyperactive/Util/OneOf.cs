namespace hyperactive {
    using System;
    using System.Diagnostics.CodeAnalysis;

    // a stupid experiment that simulates TypeScript's union types; e.g `number | string`
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Needed for overload disambugation.")]
    [SuppressMessage("Redundancy", "RCS1163:Unused parameter.", Justification = "Needed for overload disambugation.")]
    public struct OneOf<A, B>
        where A : class
        where B : class {
        private readonly A? a;
        private readonly B? b;

        public OneOf(A a) => (this.a, b) = (a, default);
        public OneOf(B b) => (a, this.b) = (default, b);

        public bool Is<T>(A? dummy = default) where T : A => a is not null;
        public bool Is<T>(B? dummy = default) where T : B => b is not null;

        public static implicit operator OneOf<A, B>(A a) => new(a);
        public static implicit operator OneOf<A, B>(B b) => new(b);

        public static implicit operator A(OneOf<A, B> o) => o.a ?? ThrowNot<A>();
        public static implicit operator B(OneOf<A, B> o) => o.b ?? ThrowNot<B>();

        private static T ThrowNot<T>() => throw new InvalidOperationException($"Did not contain a {typeof(T).Name} value.");
    }
}
