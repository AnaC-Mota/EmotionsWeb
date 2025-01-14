import {APIService}  from "../http-common";

class EmotionService{
    create(data){
        return APIService.Axios().post("Home/AddDocument", data);
    }
    getdocuments(){
        return APIService.Axios().post("Home/GetAllDocuments");
    }
    post(){
        return APIService.Axios().post("Grafico");
    }
}

export default new EmotionService();