import axios from "axios"


class APIServiceClass {
    instance = axios.create(
        {baseURL: "https://localhost:7237/"}
    )

    constructor(){
        this.instance.interceptors.request.use(
            (config) => {
                const token = localStorage.getItem("authToken")
                if(token){
                    console.log(token)
                    config.headers.Authorization = `Bearer ${token}`
                }
                return config
            },
            (error) => {
                return error
            })
        }
        Axios() {
            return this.instance;
        }
        
    }
    
    export const APIService = new APIServiceClass();
