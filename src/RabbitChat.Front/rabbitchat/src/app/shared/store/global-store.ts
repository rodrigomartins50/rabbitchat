import { BehaviorSubject } from 'rxjs';
import { MessageChat } from 'src/app/dto/messageChat';
import { StateConnectionEnum } from '../enum/state-connection.enum';

export class GlobalStore {

    private _connectionState$ = new BehaviorSubject<StateConnectionEnum>(StateConnectionEnum.fechado);
    public connectionState$ = this._connectionState$.asObservable();

    private _messages$ = new BehaviorSubject<MessageChat[]>([]);
    public messages$ = this._messages$.asObservable();

    private _username$ = new BehaviorSubject<string>("");
    public username$ = this._username$.asObservable();

    getConnectionState(): StateConnectionEnum {
        return this._connectionState$.getValue();
    }

    setConnectionState(connectionState: StateConnectionEnum) {
        this._connectionState$.next(connectionState);
    }

    getMessages(): MessageChat[] {
        return this._messages$.getValue();
    }

    getUsername(): string {
        return this._username$.getValue();
    }

    setUsername(username: string) {
        this._username$.next(username);
    }
}
