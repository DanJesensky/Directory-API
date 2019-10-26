namespace Directory {
    /// <summary>
    /// Static class containing global constants with various uses throughout the projects.
    /// </summary>
    public static class Constants {
        /// <summary>
        /// Configuration keys
        /// </summary>
        public static class Config {
            /// <summary>
            /// The configuration key for the picture to use for people who have not added one of their own.
            /// </summary>
            public const string DefaultPictureFileKey = "DefaultPictureLocation";

            /// <summary>
            /// The configuration key for the database connection string.
            /// </summary>
            public const string DatabaseConnectionString = "DatabaseConnectionString";
        }

        /// <summary>
        /// Scopes used by the application and issued by the authorization server
        /// </summary>
        public static class Scopes {
            /// <summary>
            /// The base scope for the directory. Grants access to edit themselves, as well as any information the
            /// directory keeps separately from the authorization server, e.g. email notification preferences
            /// if there are any implemented.
            /// </summary>
            public const string Base = "directory";

            /// <summary>
            /// The administration scope for the directory. Grants access to edit other people, as well as creation and
            /// hiding (soft deletion) of new people.
            /// </summary>
            public const string Administrator = "directory.administrator";
        }

        /// <summary>
        /// Authorization policies used in the application.
        /// </summary>
        public static class AuthorizationPolicies {
            /// <summary>
            /// The default policy, requiring only an authenticated principal.
            /// </summary>
            public const string DefaultPolicy = "DefaultPolicy";
        }

        /// <summary>
        /// The location of the default built-in default picture. This resource is compiled into the library,
        /// so the format must be something Assembly.GetManifestResourceStream(string) can resolve.
        /// </summary>
        public const string DefaultBuiltInPictureLocation = "Directory.DefaultBrother.png";
    }
}
