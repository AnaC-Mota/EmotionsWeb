import { useState, useEffect, Component } from "react";
import { Route, Navigate } from "react-router-dom";

function PrivateRouter({ children, ...rest }) {
    const [isAuthorized, setIsAuthorized] = useState(null)

    useEffect(() => {
        auth().catch(()=>{
            setIsAuthorized(false)
        })
    }, [])

    const auth = async () => {
        const token = localStorage.getItem("authToken")
        if(!token){
            setIsAuthorized(false)
        }
        else{
            setIsAuthorized(true)
        }
    }

    if (isAuthorized == null){
        return <div>Loading...</div>
    }

    return isAuthorized ? children : <Navigate to="/"></Navigate>
}

export default PrivateRouter;
