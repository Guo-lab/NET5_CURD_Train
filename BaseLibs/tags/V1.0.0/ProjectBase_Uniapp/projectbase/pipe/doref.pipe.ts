import { DORef } from 'projectbase/projectbase.type';

export class DORefPipe{
    static transform(value: any, list: DORef[]) {
        return list.find((doref) => doref.id === value).refText;
    }

}
