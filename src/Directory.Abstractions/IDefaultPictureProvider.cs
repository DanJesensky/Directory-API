using System;

namespace Directory.Abstractions {
    public interface IDefaultPictureProvider {
        /// <summary>
        /// Gets the byte array that represents the default picture of brothers that have not added one of their own.
        /// </summary>
        /// <returns>The byte array that represents the default picture.</returns>
        byte[] GetDefaultPicture();
    }
}
