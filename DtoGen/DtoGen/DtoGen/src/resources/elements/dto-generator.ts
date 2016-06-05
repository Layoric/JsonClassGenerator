import {bindable, autoinject} from "aurelia-framework";
import {HttpClient} from "aurelia-http-client";

@autoinject()
export class DtoGeneratorCustomElement {
    result: string;
    @bindable json: string;

    constructor(private httpClient: HttpClient) {
        console.log('helllooooo');
        this.httpClient.configure(config => {
            config.withHeader('Accept', 'application/json');
        });
    }

    jsonChanged(newValue) {
        console.log('changed1');
        if (newValue.length > 0) {
            this.httpClient.post('/json/csharp', { Json: this.json }).then((response) => {
                this.result = response.content.Dtos;
            });
        } else {
            this.result = '';
        }
    }
}