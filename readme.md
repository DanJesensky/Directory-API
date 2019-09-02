# Directory API

This is the C# implementation of the API for the Lambda Chi Alpha [Kappa-Phi chapter Directory](https://kappa-phi.com).

It is at the point where it is feature-paired with the original java implementation. This was meant to be an "MVP" state, and so some things were neglected (e.g. comments) but will come soon after.

The goal of this project was mainly to escape the new Java licensing structure (and to avoid having to deal with OpenJDK, since developing against Oracle's JDK and then using OpenJDK can produce odd bugs).

With feature parity out of the way, new features can also be built (right now create and read are supported with a couple of update operations). Update of most things besides pictures and brother details still needs to be done, along with deletion of everything.
