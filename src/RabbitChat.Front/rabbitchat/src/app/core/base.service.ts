import { HttpHeaders, HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export abstract class BaseService {
    protected urlService: string = environment.urlService;

    constructor(private http: HttpClient) { }

    protected ObterHeaderJson(typeJson: boolean, reportProgress: boolean, responseTypeBlob: boolean): any {

        let cabecalho:any = {
            'Content-Type': 'application/json;charset=utf-8',
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Credentials': 'true'
        };

        if(typeJson){
            cabecalho['Content-Type'] = 'application/json;charset=utf-8'
        }

        let responseTyped = responseTypeBlob ? "blob": "json";

        return {
            headers: new  HttpHeaders(cabecalho),
            responseType: responseTyped,
            reportProgress: reportProgress
        }
    }

    protected get(url:string):Observable<any>{
        return this.http.get<any>(this.urlService + url, this.ObterHeaderJson(true, false, false))
        .pipe(
            catchError(error =>{
                 return this.handleErros(error);
            })
        );
    }

    protected post(url:string, sendObject:any):Observable<any>{
        return this.http.post<any>(this.urlService + url, sendObject, this.ObterHeaderJson(true, false, false))
        .pipe(
            catchError(error =>{
                 return this.handleErros(error);
            })
        );
    }

    handleErros(error: any):  Observable<never> {
        if (error instanceof HttpErrorResponse) {

            alert("Ocorreu um erro inesperado.");
        }

        return throwError(error);
    }

}