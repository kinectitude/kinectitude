// Learn more about F# at http://fsharp.net

module Module1

open Kinectitude.Core.Base
open Kinectitude.Core.Attributes
open Kinectitude.Core.Data

type FsComponent() =
    inherit Component()
    let mutable matcher : ITypeMatcher = null

    [<Plugin("test type matcher", "")>]
    member public this.Type
        with get() : ITypeMatcher = matcher
        and set(value : ITypeMatcher) =
            (if matcher <> value then
                matcher <- value
                this.Change("Type"))

    override this.OnUpdate(frameDelta : float32) = ()
    override this.Destroy() = ()
