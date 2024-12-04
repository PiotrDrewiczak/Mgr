import { Point } from "../../polygon/models/point.model";

export interface PolygonWithCenter {
    x: number;
    y: number;
    vertices: Point[];
  }