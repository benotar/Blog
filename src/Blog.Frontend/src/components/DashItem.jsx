import {useEffect} from "react";
import $axios from "../axios/axios.js";
import {useAppStore} from "../zustand/useAppStore.js";
import {Button, Modal, Table} from "flowbite-react";
import {HiOutlineExclamationCircle} from "react-icons/hi";
import {ADMIN} from "../shared/utils.js";

const DashItem = ({
                      fetchUrl,
                      setItems,
                      setShowMore,
                      itemsLength,
                      showMoreUrl,
                      setShowModal,
                      urlToDelete,
                      itemIdToDelete,
                      showModal,
                      showMore,
                      isComments = false,
                      children
                  }) => {

    const currentUser = useAppStore((state) => state.currentUser);

    useEffect(() => {

        const fetchItems = async () => {
            try {
                const {data} = await $axios.get(fetchUrl);
                if (data.isSucceed) {
                    setItems(isComments ? data.payload.items : data.payload.data.items);
                    if (isComments
                        ? (!data.payload.hasNextPage)
                        : (!data.payload.data.hasNextPage)) {
                        setShowMore(false);
                    }
                }
            } catch (error) {
                console.log(error.message);
            }
        }

        if (currentUser.role === ADMIN) {
            fetchItems();
        }
    }, [currentUser.role]);

    const handleShowMore = async () => {
        try {
            const {data} = await $axios.get(showMoreUrl);
            if (data.isSucceed) {
                setItems((prev) => isComments ? [...prev, ...data.payload.items] : [...prev, ...data.payload.data.items]);
                if (isComments
                    ? (!data.payload.hasNextPage)
                    : (!data.payload.data.hasNextPage)) {
                    setShowMore(false);
                }
            }
        } catch (error) {
            console.log(error.message);
        }
    }

    const handleDeleteRecord = async () => {
        setShowModal(false);
        try {

            const {data} = await $axios.delete(urlToDelete);

            if (data.isSucceed) {
                setItems((prevItems) => prevItems.filter(item => item.id !== itemIdToDelete));
            }
        } catch (error) {
            console.log(error.message);
        }
    }

    return (
        <div className="table-auto overflow-x-scroll md:mx-auto p-3 scrollbar
        scrollbar-track-slate-100 scrollbar-thumb-slate-300 dark:scrollbar-track-slate-700 dark:scrollbar-thumb-slate-500">
            {
                currentUser.role === ADMIN && itemsLength > 0
                    ? (
                        <>
                            <Table
                                hoverable
                                className="shadow-md"
                            >
                                {children}
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
                        <p>You have no records yet!</p>
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
                            Are you sure you want to delete this record?
                        </h3>
                        <div className="flex justify-center gap-4">
                            <Button
                                color="failure"
                                onClick={handleDeleteRecord}
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
};

export default DashItem;