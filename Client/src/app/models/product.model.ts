export interface Product {
    id: number;
    name: string;
    sku: string;
    barcode?: string;
    categoryId: number;
    categoryName: string;
    size?: string;
    color?: string;
    fabric?: string;
    purchasePrice: number;
    salePrice: number;
    mrp: number;
    taxPercentage: number;
    isActive: boolean;
}

export interface Category {
    id: number;
    name: string;
}
