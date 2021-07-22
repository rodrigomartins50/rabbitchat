import { BehaviorSubject } from 'rxjs';
import { StateConnectionEnum } from '../enum/state-connection.enum';

export class GlobalStore {

    private _connectionState$ = new BehaviorSubject<StateConnectionEnum>(StateConnectionEnum.fechado);
    public connectionState$ = this._connectionState$.asObservable();

    getConnectionState(): StateConnectionEnum {
        return this._connectionState$.getValue();
    }

    setConnectionState(connectionState: StateConnectionEnum) {
        this._connectionState$.next(connectionState);
    }
}
