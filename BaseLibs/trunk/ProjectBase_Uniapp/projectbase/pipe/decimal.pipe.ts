export class DecimalPipe{
    static DefaultFracSize = 2;
    static transform(value: number, fracSize?: number) {
        fracSize = fracSize ||DecimalPipe.DefaultFracSize;
        return value.toFixed(fracSize);
    }
}
