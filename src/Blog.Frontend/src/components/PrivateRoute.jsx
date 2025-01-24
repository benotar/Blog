import {useAppStore} from "../zustand/useAppStore.js";
import {Outlet, Navigate} from "react-router-dom";

export default function PrivateRoute() {

    const currentUser = useAppStore((state) => state.currentUser);

    return currentUser ? <Outlet/> : <Navigate to="/sign-in"/>;
}