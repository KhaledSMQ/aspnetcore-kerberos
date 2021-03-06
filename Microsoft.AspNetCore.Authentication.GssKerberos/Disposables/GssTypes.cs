﻿using System;
using System.Text;
using static Microsoft.AspNetCore.Authentication.GssKerberos.Native.Krb5Interop;

namespace Microsoft.AspNetCore.Authentication.GssKerberos.Disposables
{
    internal static class GssBuffer
    {
        private static readonly Encoding iso8859 = Encoding.GetEncoding("iso-8859-1");

        internal static Disposable<GssBufferStruct> FromString(string buffer) =>
            FromBytes(iso8859.GetBytes(buffer));

        public static Disposable<GssBufferStruct> FromBytes(byte[] buffer) =>
            Disposable.From(
                Pinned.From(buffer), p => new GssBufferStruct
                {
                    length = (uint)p.Value.Length,
                    value = p.Address
                }, p =>
                {
                    var majorStatus = gss_release_buffer(out var minorStatus, ref p);
                    if (majorStatus != GSS_S_COMPLETE)
                        throw new GssException("An error occurred releasing a buffer allocated by the GSS provider", 
                            majorStatus, minorStatus, GSS_C_NO_OID);
                });
    }
}