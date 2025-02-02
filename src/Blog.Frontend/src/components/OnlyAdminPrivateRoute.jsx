import {useAppStore} from "../zustand/useAppStore.js";
import {Outlet, Navigate} from "react-router-dom";

const OnlyAdminPrivateRoute = () => {

    const currentUser = useAppStore((state) => state.currentUser);

    return currentUser && currentUser.role === "Admin" ? <Outlet/> : <Navigate to="/sign-in"/>;
};

export default OnlyAdminPrivateRoute;