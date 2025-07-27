class Cola
    {
    constructor(){
        this.cola = [];
    }

    encolar(){
        if (arguments.length === 0) {
            return;
        }
        for (let i = 0; i < arguments.length; i++) {
            this.cola.push(arguments[i]);
        }
    }

    encolarInicio(elemento) {
        this.cola.unshift(elemento);
    }

    desencolar(){
        if (this.cola.length === 0) {
            return null;
        }
        return this.cola.shift();
    }

    obtenerPrimero(){
        if (this.cola.length === 0) {
            return null;
        }
        return this.cola[0];
    }

    estaVacia(){
        return this.cola.length === 0;
    }

    tamano(){
        return this.cola.length;
    }

    vaciar(){
        this.cola = [];
    }

}

