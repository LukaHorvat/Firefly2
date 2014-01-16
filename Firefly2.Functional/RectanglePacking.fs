namespace Firefly2.Functional.RectanglePacking

type IRectangle =
    abstract member X : double with get, set
    abstract member Y : double with get, set
    abstract member Width : double
    abstract member Height : double
    abstract member Id : int

type internal Spot(x: double, y: double, width: double, height: double) = 
    member this.x = x
    member this.y = y
    member this.width = width
    member this.height = height

    member this.cut = fun (rect: IRectangle) ->
        let intervalIntersect = 
            fun start1 end1 start2 end2 -> min (end1 - start2) (end2 - start1) > 0.0;

        let horizontalIntersect = intervalIntersect x (x + width) rect.X (rect.X + rect.Width)
        let verticalIntersect   = intervalIntersect y (y + height) rect.Y (rect.Y + rect.Height)

        if (horizontalIntersect && rect.Y >= y) then
            Spot(x, y, width, min (rect.Y - y) height)
        elif (verticalIntersect && rect.X >= x) then
            Spot(x, y, min (rect.X - x) width, height)
        else this

module Packer =
    let private initialSpots = [Spot(0.0, 0.0, System.Double.MaxValue, System.Double.MaxValue)]

    let private bestSpot (rect: IRectangle) (spots: Spot list) =
        spots
        |> Seq.filter (fun spot -> spot.width >= rect.Width && spot.height >= rect.Height)
        |> Seq.sortBy (fun spot -> max (spot.x + rect.Width) (spot.y + rect.Height))
        |> Seq.head

    let private putRectangle (rect: IRectangle) (spots: Spot list): Spot list =
        let best = bestSpot rect spots
        rect.X <- best.x
        rect.Y <- best.y
        let right = Spot(best.x + rect.Width, best.y, best.width - rect.Width, best.height)
        let top = Spot(best.x, best.y + rect.Height, best.width, best.height - rect.Height)
        [
        for spot in spots do 
            if spot <> best then 
                yield spot.cut rect
        yield right
        yield top]

    let rec private putRectangles (rectangles: IRectangle list) (spots: Spot list) =
        match rectangles, spots with
        | [], _ -> ()
        | rect::rest, _ -> putRectangles rest (putRectangle rect spots)

    let packRectangles (rectangles: IRectangle list) = 
        let ordered = List.sortBy (fun (rect: IRectangle) -> -(rect.Width * rect.Height)) rectangles
        putRectangles ordered initialSpots
        ordered

        