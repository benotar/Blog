import {useAppStore} from "../zustand/useAppStore.js";
import {useEffect, useState} from "react";
import $axios from "../axios/axios.js";
import {Button, Modal, Table} from "flowbite-react";
import {HiOutlineExclamationCircle} from "react-icons/hi";
import {FaCheck} from "react-icons/fa";


const DashUsers = () => {
    const currentUser = useAppStore((state) => state.currentUser);
    const [items, setItems] = useState([]);
    const [showMore, setShowMore] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [userIdToDelete, setUserIdToDelete] = useState(null);

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const {data} = await $axios.get("user/get-users");
                if (data.isSucceed) {
                    setItems(data.payload.data.items);
                    if(!data.payload.data.hasNextPage) {
                        setShowMore(false);
                    }
                }
            } catch (error) {
                console.log(error.message);
            }
        };

        if (currentUser.role === "Admin") {
            fetchUsers();
        }
    }, [currentUser.role]);

    const handleShowMore = async () => {
        const startIndex = items.length;
        try {
            const {data} = await $axios.get(`user/get-users?startIndex=${startIndex}`);
            if (data.isSucceed) {
                setItems((prev) => [...prev, ...data.payload.data.items]);
                if(!data.payload.data.hasNextPage) {
                    setShowMore(false);
                }
            }
        } catch (error) {
            console.log(error.message);
        }
    }

    const handleDeleteUser = async () => {
        setShowModal(false);
        try {
            const {data} = await $axios.delete(`user/delete/${userIdToDelete}`);

            if (data.isSucceed) {
                setItems((prevItems) => prevItems.filter(item => item.id !== userIdToDelete));
            }
        } catch (error) {
            console.log(error.message);
        }
    }

    return (
        <div className="table-auto overflow-x-scroll md:mx-auto p-3 scrollbar
        scrollbar-track-slate-100 scrollbar-thumb-slate-300 dark:scrollbar-track-slate-700 dark:scrollbar-thumb-slate-500">
            {
                currentUser.role === "Admin" && items.length > 0
                    ? (
                        <>
                            <Table
                                hoverable
                                className="shadow-md"
                            >
                                <Table.Head>
                                    <Table.HeadCell>Date created</Table.HeadCell>
                                    <Table.HeadCell>User image</Table.HeadCell>
                                    <Table.HeadCell>Username</Table.HeadCell>
                                    <Table.HeadCell>Email</Table.HeadCell>
                                    <Table.HeadCell>Admin</Table.HeadCell>
                                    <Table.HeadCell>Delete</Table.HeadCell>
                                </Table.Head>
                                {
                                    items.map((item) =>
                                        <Table.Body
                                            className="divide-y"
                                            key={item.id}
                                        >
                                            <Table.Row
                                                className="bg-white dark:border-gray-700 dark:bg-gray-800"
                                            >
                                                <Table.Cell>
                                                    {new Date(item.createdAt).toLocaleDateString()}
                                                </Table.Cell>
                                                <Table.Cell>
                                                    <img
                                                        src={item.profilePictureUrl}
                                                        alt={item.username}
                                                        className="w-10 h-10 object-cover bg-gray-500 rounded-full"
                                                    />
                                                </Table.Cell>
                                                <Table.Cell>{item.username}</Table.Cell>
                                                <Table.Cell>{item.email}</Table.Cell>
                                                <Table.Cell>
                                                    {item.role === "Admin" ? (
                                                        <FaCheck className="text-green-500"/>
                                                    ) : (
                                                        <FaCheck className="text-red-500"/>
                                                    )}
                                                </Table.Cell>
                                                <Table.Cell>
                                                <span
                                                    onClick={() => {
                                                        setShowModal(true);
                                                        setUserIdToDelete(item.id);
                                                    }}
                                                    className="font-medium text-red-500 hover:underline cursor-pointer"
                                                >
                                                    Delete
                                                </span>
                                                </Table.Cell>
                                            </Table.Row>
                                        </Table.Body>
                                    )
                                }
                            </Table>
                            {showMore && (
                                <button
                                    onClick={handleShowMore}
                                    className="w-full text-teal-500 self-center text-sm py-7"
                                >
                                    Show more
                                </button>
                            )}
                        </>
                    )
                    : (
                        <p>You have no users yet!</p>
                    )
            }
            <Modal
                show={showModal}
                onClose={() => setShowModal(false)}
                popup
                size="md"
            >
                <Modal.Header/>
                <Modal.Body>
                    <div className="text-center">
                        <HiOutlineExclamationCircle
                            className="h-14 w-14 text-gray-400 dark:text-gray-200
                            mb-4 mx-auto"
                        />
                        <h3
                            className="mb-5 text-lg text-gray-500 dark:text-gray-400"
                        >
                            Are you sure you want to delete this user?
                        </h3>
                        <div className="flex justify-center gap-4">
                            <Button
                                color="failure"
                                onClick={handleDeleteUser}
                            >
                                Yes, I&apos;m sure
                            </Button>
                            <Button
                                color="gray"
                                onClick={() => setShowModal(false)}
                            >
                                No, cancel
                            </Button>
                        </div>
                    </div>
                </Modal.Body>
            </Modal>
        </div>
    );
}

export default DashUsers;