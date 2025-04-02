import axios from "axios";

class APIServiceClass {
    instance = axios.create({
        baseURL: "https://localhost:7237/",
        headers: {
            "Content-Type": "application/json",
        },
    });

    constructor() {
        this.instance.interceptors.request.use(
            (config) => {
                const token = localStorage.getItem("authToken"); 
                if (token) {
                    console.log("Token enviado na requisição:", token);
                    config.headers["Authorization"] = `Bearer ${token}`;
                } else {
                    console.warn("Nenhum token encontrado no localStorage.");
                }
                return config;
            },
            (error) => {
                return Promise.reject(error);
            }
        );
    }

    Axios() {
        return this.instance;
    }
}

export const APIService = new APIServiceClass();
