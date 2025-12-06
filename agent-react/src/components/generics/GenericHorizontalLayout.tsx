import { ComponentType } from "react";

interface GenericHorizontalLayoutProps<T = any> {
  items: T[];
  ItemComponent: ComponentType<{ item: T }>;
  getKey?: (item: T, index: number) => string | number;
}

export default function GenericHorizontalLayout<T>({ 
  items, 
  ItemComponent,
  getKey = (_, index) => index
}: GenericHorizontalLayoutProps<T>) {
  return (
    <div className="flex gap-2 text-sm">
      {items.map((item, index) => (
        <ItemComponent key={getKey(item, index)} item={item} />
      ))}
    </div>
  );
}
