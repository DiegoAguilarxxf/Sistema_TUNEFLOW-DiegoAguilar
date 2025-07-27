class Pila
{
    constructor() {
        this.pila = [];
    }

    apilar() {
        if (arguments.length === 0) {
            console.error("No se han proporcionado elementos para apilar.");
            return;
        }
        for (let i = 0; i < arguments.length; i++) {
            this.pila.push(arguments[i]);
        }
    }

    desapilar() {
        if (this.pila.length === 0) {
            console.error("La pila está vacía. No se puede desapilar.");
            return null;
        }
        return this.pila.pop();
    }

    cima() {
        if (this.pila.length === 0) {
            console.error("La pila está vacía. No hay elemento en la cima.");
            return null;
        }
        return this.pila[this.pila.length - 1];
    }
    estaVacia() {
        return this.pila.length === 0;
    }
    tamano() {
        return this.pila.length;
    }
    vaciar() {
        this.pila = [];
    }

}

